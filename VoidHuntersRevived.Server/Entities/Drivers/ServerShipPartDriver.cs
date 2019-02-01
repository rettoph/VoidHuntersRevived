using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Connections.Nodes;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Server.Helpers;
using VoidHuntersRevived.Server.Scenes;

namespace VoidHuntersRevived.Server.Entities.Drivers
{
    public class ServerShipPartDriver : ServerFarseerEntityDriver
    {
        private ShipPart _parent;
        private ServerMainScene _scene;

        public ServerShipPartDriver(ShipPart parent, EntityInfo info, IGame game) : base(parent, info, game)
        {
            _parent = parent;
            _parent.OnInitialize += this.HandleParentInitialize;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _scene = this.Scene as ServerMainScene;
        }

        private void HandleParentInitialize(object sender, IInitializable e)
        {
            _parent.MaleConnectionNode.OnConnected += this.HandleMaleNodeConnection;
            _parent.MaleConnectionNode.OnDisconnected += this.HandleMaleNodeDisconneced;
        }

        /// <summary>
        /// When a new connection is made we must alert all clients.
        /// this will construct a message and push it to all 
        /// connections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMaleNodeConnection(object sender, ConnectionNode e)
        {
            // Send the message to all clients
            _scene.Group.SendMessage(
                ServerMessageHelper.BuildCreateNodeConnectionMessage(e.Connection, _scene.Group),
                NetDeliveryMethod.ReliableOrdered,
                0);
        }

        private void HandleMaleNodeDisconneced(object sender, ConnectionNode e)
        {
            throw new NotImplementedException();
        }
    }
}
