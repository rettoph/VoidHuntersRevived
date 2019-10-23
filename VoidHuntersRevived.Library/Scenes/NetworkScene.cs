using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Utilities.Delegater;
using Guppy;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guppy.Network.Utilitites.Delegaters;

namespace VoidHuntersRevived.Library.Scenes
{
    public abstract class NetworkScene : Scene
    {
        #region Private Fields
        private Interval _intervals;
        private ConcurrentQueue<NetIncomingMessage> _actions;
        private NetIncomingMessage _im;

        private List<Single> _actionsPerFrame;
        private Single _actionsThisFrame;
        #endregion

        #region Internal Attributes
        internal GroupMessageDelegater actions;
        #endregion

        #region Public Attributes
        public Group Group { get; set; }

        public Single ActionsPerFrame { get => _actionsPerFrame.Sum() / _actionsPerFrame.Count(); }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _intervals = provider.GetRequiredService<Interval>();
            _actions = new ConcurrentQueue<NetIncomingMessage>();
            _actionsPerFrame = new List<Single>();

            this.actions = provider.GetRequiredService<GroupMessageDelegater>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.actions.Group = this.Group;

            this.Group.Messages.TryAdd("entity:action", this.HandleNetworkEntityActionMessage);
        }

        public override void Dispose()
        {
            base.Dispose();

            this.Group.Messages.TryRemove("entity:action", this.HandleNetworkEntityActionMessage);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            _intervals.Update(gameTime);
            this.Group.Messages.ReadAll();

            base.Update(gameTime);

            this.Group.Messages.SendAll();
            this.actions.SendAll();

            while (_actions.Any())
                if (_actions.TryDequeue(out _im))
                    this.entities.GetById<NetworkEntity>(_im.ReadGuid())?.Actions.TryInvoke(this, _im.ReadString(), _im);

            _actionsPerFrame.Add(_actionsThisFrame);
            _actionsThisFrame = 0;

            while (_actionsPerFrame.Count > 100)
                _actionsPerFrame.RemoveAt(0);
        }
        #endregion

        #region Message Handlers
        private void HandleNetworkEntityActionMessage(object sender, NetIncomingMessage arg)
        {
            _actions.Enqueue(arg);
            _actionsThisFrame++;   
        }
        #endregion
    }
}
