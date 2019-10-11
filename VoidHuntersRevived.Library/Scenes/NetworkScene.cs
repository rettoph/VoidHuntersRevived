using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Utilities;
using GalacticFighters.Library.Utilities.Delegater;
using Guppy;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticFighters.Library.Scenes
{
    public abstract class NetworkScene : Scene
    {
        #region Private Fields
        private List<Double> _frames;
        private List<Single> _actionsPerFrame;
        private Single _actionsThisFrame;
        private Interval _intervals;
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
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            _frames = new List<Double>();
            _actionsPerFrame = new List<Single>();

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
            this.Group.TryUpdate(gameTime);

            base.Update(gameTime);

            this.Frames++;
            this.Seconds = gameTime.TotalGameTime.TotalSeconds;
            _frames.Add(gameTime.ElapsedGameTime.TotalSeconds);

            _actionsPerFrame.Add(_actionsThisFrame);
            _actionsThisFrame = 0;

            while (_frames.Count() > 5)
                _frames.RemoveAt(0);

            while (_actionsPerFrame.Count() > 5000)
                _actionsPerFrame.RemoveAt(0);
        }
        #endregion

        #region Message Handlers
        private void HandleNetworkEntityActionMessage(object sender, NetIncomingMessage arg)
        {
            _actionsThisFrame++;
            this.ActionsRecieved++;
            this.entities.GetById<NetworkEntity>(arg.ReadGuid())?.Actions.TryInvoke(this, arg.ReadString(), arg);
        }
        #endregion
    }
}
