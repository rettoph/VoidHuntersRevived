using GalacticFighters.Library.Entities;
using Guppy;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Scenes
{
    public abstract class NetworkScene : Scene
    {
        #region Public Attributes
        public Group Group { get; set; }

        public Single ActionsRecieved { get; private set; }
        public Single Frames { get; private set; }
        public Double Seconds { get; private set; }
        public Single ActionsPerFrame { get => this.ActionsRecieved / this.Frames; }
        public Double ActionsPerSecond { get => this.ActionsRecieved / this.Seconds; }
        public Double FramesPerSecond { get => this.Frames / this.Seconds; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

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
            this.Group.TryUpdate(gameTime);

            base.Update(gameTime);

            this.Frames++;
            this.Seconds = gameTime.TotalGameTime.TotalSeconds;
        }
        #endregion

        #region Message Handlers
        private void HandleNetworkEntityActionMessage(object sender, NetIncomingMessage arg)
        {
            this.ActionsRecieved++;
            this.entities.GetById<NetworkEntity>(arg.ReadGuid())?.Actions.TryInvoke(this, arg.ReadString(), arg);
        }
        #endregion
    }
}
