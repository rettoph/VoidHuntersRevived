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
        private List<Double> _frames;
        private List<Single> _actionsPerFrame;
        private Single _actionsThisFrame;
        private Interval _intervals;
        private ConcurrentQueue<NetIncomingMessage> _actions;
        private NetIncomingMessage _im;
        #endregion

        #region Internal Attributes
        internal GroupMessageDelegater actions;
        #endregion

        #region Public Attributes
        public Group Group { get; set; }
        public Single ActionsRecieved { get; private set; }
        public Single Frames { get; private set; }
        public Double Seconds { get; private set; }
        public Single AverageActionsPerFrame { get => this.ActionsRecieved / this.Frames; }
        public Double AverageActionsPerSecond { get => this.ActionsRecieved / this.Seconds; }
        public Double AverageFramesPerSecond { get => this.Frames / this.Seconds; }
        public Double FramesPerSecond { get => 1 / (_frames.Sum() / _frames.Count); }
        public Double ActionsPerSecond { get => (_actionsPerFrame.Sum() / _actionsPerFrame.Count) * this.FramesPerSecond; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _intervals = provider.GetRequiredService<Interval>();
            _actions = new ConcurrentQueue<NetIncomingMessage>();

            this.actions = provider.GetRequiredService<GroupMessageDelegater>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            _frames = new List<Double>();
            _actionsPerFrame = new List<Single>();

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

            this.Frames++;
            this.Seconds = gameTime.TotalGameTime.TotalSeconds;
            _frames.Add(gameTime.ElapsedGameTime.TotalSeconds);

            _actionsPerFrame.Add(_actionsThisFrame);
            _actionsThisFrame = 0;

            while (_frames.Count() > 5)
                _frames.RemoveAt(0);

            while (_actionsPerFrame.Count() > 100)
                _actionsPerFrame.RemoveAt(0);
        }
        #endregion

        #region Message Handlers
        private void HandleNetworkEntityActionMessage(object sender, NetIncomingMessage arg)
        {
            _actionsThisFrame++;
            this.ActionsRecieved++;
            _actions.Enqueue(arg);
            
        }
        #endregion
    }
}
