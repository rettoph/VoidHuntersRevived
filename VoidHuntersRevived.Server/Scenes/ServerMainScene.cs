using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Networking.Groups;
using VoidHuntersRevived.Networking.Interfaces;
using VoidHuntersRevived.Networking.Peers;
using Lidgren.Network.Xna;

namespace VoidHuntersRevived.Server.Scenes
{
    public class ServerMainScene : MainScene, IDataHandler
    {
        public Wall Wall { get; protected set; }

        private ServerPeer _server;
        private ServerGroup _group;

        public ServerMainScene(IPeer peer, IServiceProvider provider, IGame game) : base(provider, game)
        {
            _server = peer as ServerPeer;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _group = _server.Groups.GetById(69) as ServerGroup;
            _group.DataHandler = this;
            this.Group = _group;

            // Create and setup a new wall
            this.Wall = this.Entities.Create<Wall>("entity:wall");
            this.Wall.Configure(5, 5);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the internal group
            _group.Update();
        }

        public void HandleData(NetIncomingMessage data)
        {
            throw new NotImplementedException();
        }

        public void HandleUserJoined(IUser user, NetConnection connection = null)
        {
            // Send the user the current world state...
            var om = this.Group.CreateMessage();
            om.Write((Byte)DataAction.Configure);
            om.Write(this.World.Gravity);
            this.Wall.Create(om);

            // Update the client with the basic world data
            _server.SendMessage(om, connection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void HandleUserLeft(IUser user, NetConnection connection = null)
        {
            throw new NotImplementedException();
        }
    }
}
