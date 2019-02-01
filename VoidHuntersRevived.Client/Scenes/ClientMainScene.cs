using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Entities;
using VoidHuntersRevived.Client.Layers;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Networking.Groups;
using VoidHuntersRevived.Networking.Interfaces;
using VoidHuntersRevived.Networking.Peers;

namespace VoidHuntersRevived.Client.Scenes
{
    public class ClientMainScene : MainScene
    {
        public Camera Camera { get; set; }
        public Cursor Cursor { get; set; }

        // The current clients player object
        public UserPlayer CurrentPlayer { get; set; }

        private GraphicsDevice _grapihcs;

        private ClientPeer _client;
        private ClientGroup _group;

        public ClientMainScene(IPeer peer, GraphicsDevice graphics, IServiceProvider provider, IGame game) : base(provider, game)
        {
            _client = peer as ClientPeer;
            _grapihcs = graphics;

            this.Visible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _group = _client.Groups.GetById(69) as ClientGroup;
            this.Group = _group;

            this.Group.MessageTypeHandlers.Add("setup", this.HandleSetupMessage);
            this.Group.MessageTypeHandlers.Add("setup:complete", this.HandleSetupCompleteMessage);
            this.Group.MessageTypeHandlers.Add("create", this.HandleCreateMessage);
            this.Group.MessageTypeHandlers.Add("create:node-connection", this.HandleCreateNodeConnectionMessage);
            

            var layer = this.Layers.Create<FarseerEntityLayer>();
            this.Entities.SetDefaultLayer(layer);

            // Create the basic global entities
            this.Cursor = this.Entities.Create<Cursor>("entity:cursor");
            this.Camera = this.Entities.Create<Camera>("entity:camera");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the internal group
            _group.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            _grapihcs.Clear(Color.Black);

            base.Draw(gameTime);
        }

        #region MessageType Handlers
        private void HandleSetupMessage(NetIncomingMessage im)
        {
            // Remove any message type handlers we dont want pre supet completion
            this.Group.MessageTypeHandlers.Remove("update");

            this.World.Gravity = im.ReadVector2();
        }

        /// <summary>
        /// Used to create an entity when commanded by the server
        /// </summary>
        /// <param name="obj"></param>
        private void HandleCreateMessage(NetIncomingMessage im)
        {
            INetworkEntity entity = this.Entities.Create<INetworkEntity>(im.ReadString(), null, im.ReadInt64());
            entity.Read(im);
        }

        /// <summary>
        /// Handles incoming update messages
        /// </summary>
        /// <param name="im"></param>
        private void HandleUpdateMessage(NetIncomingMessage im)
        {
            var entity = this.NetworkEntities.GetById(im.ReadInt64());
            entity.Read(im);
        }

        /// <summary>
        /// Handle the status complete message
        /// </summary>
        /// <param name="im"></param>
        private void HandleSetupCompleteMessage(NetIncomingMessage im)
        {
            // Add message handlers that matter now
            this.Group.MessageTypeHandlers.Add("update", this.HandleUpdateMessage);
        }

        /// <summary>
        /// Handle a create node connection message
        /// </summary>
        /// <param name="im"></param>
        private void HandleCreateNodeConnectionMessage(NetIncomingMessage im)
        {
            var malePartId = im.ReadInt64();
            var femalePartId = im.ReadInt64();
            var femaleNodeIndex = im.ReadInt32();

            var malePart = this.NetworkEntities.GetById(malePartId) as ShipPart;
            var femalePart = this.NetworkEntities.GetById(femalePartId) as ShipPart;

            // Create the connection
            malePart.AttatchTo(femalePart.FemaleConnectionNodes[femaleNodeIndex]);
        }
        #endregion
    }
}
