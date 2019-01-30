using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Networking.Groups;
using VoidHuntersRevived.Networking.Interfaces;
using VoidHuntersRevived.Networking.Peers;
using Lidgren.Network.Xna;
using VoidHuntersRevived.Server.Helpers;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Entities.Interfaces;
using System.Linq;

namespace VoidHuntersRevived.Server.Scenes
{
    public class ServerMainScene : MainScene
    {
        public Wall Wall { get; protected set; }

        private ServerPeer _server;
        private ServerGroup _group;

        public ServerMainScene(IPeer peer, IServiceProvider provider, IGame game) : base(provider, game)
        {
            _server = peer as ServerPeer;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.NetworkEntities.OnAdd += this.HandleNetworkEntityAdd;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _group = _server.Groups.GetById(69) as ServerGroup;
            this.Group = _group;

            // Setup message handlers
            this.Group.MessageTypeHandlers.Add("update", this.HandleUpdateMessage);

            // Add group events
            this.Group.Users.OnAdd += this.HandlerUserJoined;

            // Create and setup a new wall
            this.Wall = this.Entities.Create<Wall>("entity:wall");
            this.Wall.Configure(10, 10);

            var rand = new Random();
            for(Int32 i=0; i<10; i++)
            {
                var e = this.Entities.Create<Hull>("entity:hull:square");
                e.Driver.Position = new Vector2((float)(rand.NextDouble() * 10) - 5, (float)(rand.NextDouble() * 10) - 5);
                e.Driver.LinearVelocity = new Vector2((float)(rand.NextDouble() * 2) - 1, (float)(rand.NextDouble() * 2) - 1);

                e.Driver.Rotation = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
                e.Driver.AngularVelocity = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the internal group
            _group.Update();
        }

        private void HandlerUserJoined(object sender, IUser e)
        {
            // When a user joins we will send them a quick update about the current state of
            // the scene
            var om = this.Group.CreateMessage("setup");
            om.Write(this.World.Gravity);
            _group.SendMessage(om, e, NetDeliveryMethod.ReliableOrdered, 0);

            // Update the client of every single existing entity in the world
            foreach(INetworkEntity ne in this.NetworkEntities)
            {
                _group.SendMessage(
                    ServerMessageHelper.BuildCreateMessage(ne, _group),
                    e,
                    NetDeliveryMethod.ReliableOrdered,
                    0);
            }

            // Create a new player object for the new user
            this.Entities.Create<IEntity>(
                "entity:player:user",
                null,
                e,
                this.Entities.Create<IEntity>("entity:hull:triangle", null));

            // Send a marker to the client, alerting it that setup is complete
            om = this.Group.CreateMessage("setup:complete");
            _group.SendMessage(om, e, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleNetworkEntityAdd(object sender, INetworkEntity ne)
        { // Send a create message to all connected clients
            if(_group.Users.Count() > 0)
                _group.SendMessage(
                        ServerMessageHelper.BuildCreateMessage(ne, _group),
                        NetDeliveryMethod.ReliableOrdered,
                        0);
        }

        #region Message Handlers
        /// <summary>
        /// Handles incoming update messages
        /// </summary>
        /// <param name="im"></param>
        private void HandleUpdateMessage(NetIncomingMessage im)
        {
            var entity = this.NetworkEntities.GetById(im.ReadInt64());
            entity.Read(im);
        }
        #endregion
    }
}
