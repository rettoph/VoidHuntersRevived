using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Entities;
using VoidHuntersRevived.Client.Layers;
using VoidHuntersRevived.Client.Services;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Client.Scenes
{
    public class ClientMainGameScene : MainGameScene 
    {
        #region Private Attributes
        private GraphicsDevice _graphics;
        #endregion

        #region Public Attributes
        public Camera Camera { get; protected set; }
        #endregion

        #region Constructors
        public ClientMainGameScene(
            GraphicsDevice graphics,
            IPeer peer,
            IServiceProvider provider,
            IGame game) : base(peer, provider, game)
        {
            _graphics = graphics;

            this.SetVisible(true);
        }
        #endregion

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            this.Services.Create<FarseerDebugOverlayService>();
            this.Services.Create<CameraControllerService>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Add default client specific message type handlers
            this.Group.MessageTypeHandlers.Add("update"     , this.HandleUpdateMessageType);
            this.Group.MessageTypeHandlers.Add("create"     , this.HandleCreateMessageType);
            this.Group.MessageTypeHandlers.Add("destroy"    , this.HandleDestroyMessageType);
            this.Group.MessageTypeHandlers.Add("setup:begin", this.HandleSetupBeginMessageType);
            this.Group.MessageTypeHandlers.Add("setup:end"  , this.HandleSetupEndMessageType); 
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Create a new layer and set it as the default
            var layer = this.Layers.Create<MainGameLayer>();
            this.Entities.SetDefaultLayer(layer);

            // Create a new camera for the current client
            this.Camera = this.Entities.Create<Camera>("entity:camera");
        }
        #endregion

        #region Frame Methods
        public override void Draw(GameTime gameTime)
        {
            _graphics.Clear(Color.Black);
            base.Draw(gameTime);
        }
        #endregion

        #region MessageType Handlers
        private void HandleSetupBeginMessageType(NetIncomingMessage im)
        {
            // Update the gravity value
            this.World.Gravity = im.ReadVector2();
        }

        private void HandleSetupEndMessageType(NetIncomingMessage im)
        {
            // No useful data is sent with the end message, it just acts as a marker
            // alerting the current client that setup is complete...
        }

        private void HandleCreateMessageType(NetIncomingMessage im)
        {
            // Create a brand new entity from the server sent data...
            INetworkEntity entity = this.Entities.Create<INetworkEntity>(im.ReadString(), null, im.ReadInt64());
            entity.FullRead(im);
        }

        private void HandleUpdateMessageType(NetIncomingMessage im)
        {
            // Create a brand new entity from the server sent data...
            INetworkEntity entity = this.NetworkEntities.GetById(im.ReadInt64());
            entity.Read(im);
        }

        private void HandleDestroyMessageType(NetIncomingMessage im)
        {
            // Create a brand new entity from the server sent data...
            INetworkEntity entity = this.NetworkEntities.GetById(im.ReadInt64());
            entity.Delete();
        }
        #endregion
    }
}
