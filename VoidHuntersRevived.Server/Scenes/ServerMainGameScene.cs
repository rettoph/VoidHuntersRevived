using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Networking.Interfaces;
using VoidHuntersRevived.Server.Helpers;
using Lidgren.Network.Xna;
using VoidHuntersRevived.Networking.Groups;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities.Connections;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Server.Scenes
{
    /// <summary>
    /// Server implementation of the MainGameScene
    /// </summary>
    public class ServerMainGameScene : MainGameScene
    {
        #region Private Fields
        private ServerGroup _group;
        #endregion

        #region Constructors
        public ServerMainGameScene(
            IPeer peer,
            IServiceProvider provider,
            IGame game) : base(peer, provider, game)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Add default client specific message type handlers
            this.Group.MessageTypeHandlers.Add("update", this.HandleUpdateMessageType);

            // Save the server instance of the scenes group
            _group = this.Group as ServerGroup;

            // Add all event handlers
            this.NetworkEntities.OnAdded += this.HandleNetworkEntityAdded;
            this.Group.Users.OnAdded += this.HandleUserAdded;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
        #endregion

        #region MessageType Handlers
        private void HandleUpdateMessageType(NetIncomingMessage im)
        {
            // Create a brand new entity from the server sent data...
            INetworkEntity entity = this.NetworkEntities.GetById(im.ReadInt64());
            entity.Read(im);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// The following method will handle any new incoming network entities and
        /// automatically sync those entites with any connected clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleNetworkEntityAdded(object sender, INetworkEntity e)
        {
            // Using the MessageHelper, build a create message and send it to all clients
            this.Group.SendMessage(
                ServerMessageHelper.BuildCreateNetworkEntityMessage(e, this.Group),
                NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// When a new user joins we must update them with the current world state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="user"></param>
        private void HandleUserAdded(object sender, IUser user)
        {
            // Write & send a basic setup message, with global general message details
            var om = this.Group.CreateMessage("setup:begin");
            om.Write(this.World.Gravity);
            _group.SendMessage(om, user, NetDeliveryMethod.ReliableOrdered);

            // Send the new client every existing network entity within the scene
            foreach (INetworkEntity ne in this.NetworkEntities)
                _group.SendMessage(
                    ServerMessageHelper.BuildCreateNetworkEntityMessage(ne, this.Group),
                    user,
                    NetDeliveryMethod.ReliableOrdered);

            // Create a new UserPlayer instance for the new user..
            var bridge = this.Entities.Create<ShipPart>("entity:hull:square", null);
            this.Entities.Create<UserPlayer>("entity:player:user", null, user, bridge);

            var square1 = this.Entities.Create<ShipPart>("entity:hull:square", null);
            this.Entities.Create<NodeConnection>("entity:connection:connection_node", null, square1.MaleConnectionNode, bridge.FemaleConnectionNodes[0]);

            var square2 = this.Entities.Create<ShipPart>("entity:hull:square", null);
            this.Entities.Create<NodeConnection>("entity:connection:connection_node", null, square2.MaleConnectionNode, square1.FemaleConnectionNodes[0]);

            var square3 = this.Entities.Create<ShipPart>("entity:hull:square", null);
            this.Entities.Create<NodeConnection>("entity:connection:connection_node", null, square3.MaleConnectionNode, square1.FemaleConnectionNodes[1]);

            var square4 = this.Entities.Create<ShipPart>("entity:hull:square", null);
            this.Entities.Create<NodeConnection>("entity:connection:connection_node", null, square4.MaleConnectionNode, square1.FemaleConnectionNodes[2]);

            var square5 = this.Entities.Create<ShipPart>("entity:hull:square", null);
            this.Entities.Create<NodeConnection>("entity:connection:connection_node", null, square5.MaleConnectionNode, square2.FemaleConnectionNodes[0]);

            // Send a final setup:end message, alerting the client that they have recieved all setup info
            om = this.Group.CreateMessage("setup:end");
            _group.SendMessage(om, user, NetDeliveryMethod.ReliableOrdered);
        }
        #endregion
    }
}
