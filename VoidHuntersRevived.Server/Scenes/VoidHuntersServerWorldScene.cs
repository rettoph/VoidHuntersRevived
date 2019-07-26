using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using Guppy.Network.Peers;
using Guppy.Network.Security;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Server.Scenes
{
    class VoidHuntersServerWorldScene : VoidHuntersWorldScene
    {
        private ShipCollection _ships;
        private ShipBuilder _shipBuilder;

        protected ServerPeer server;

        public VoidHuntersServerWorldScene(ShipCollection ships, ShipBuilder shipBuilder, ServerPeer server, World world, IServiceProvider provider) : base(server, world, provider)
        {
            _ships = ships;
            _shipBuilder = shipBuilder;

            this.server = server;
        }

        #region Initialization Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.Group.AddMessageHandler("chat", this.HandleChatMessage);
        }

        protected override void Initialize()
        {
            base.Initialize();

            var r = new Random();

            for(Int32 i=0; i<100; i++)
            {
                var e = this.entities.Create<ShipPart>("entity:ship-part:hull:triangle");
                e.Position = new Vector2((Single)((r.NextDouble() * 100) - 50), (Single)((r.NextDouble() * 100) - 50));
                e.Rotation = (Single)((r.NextDouble() * 10) - 5);

                e = this.entities.Create<ShipPart>("entity:ship-part:hull:square");
                e.Position = new Vector2((Single)((r.NextDouble() * 100) - 50), (Single)((r.NextDouble() * 100) - 50));
                e.Rotation = (Single)((r.NextDouble() * 10) - 5);

                e = this.entities.Create<ShipPart>("entity:ship-part:hull:hexagon");
                e.Position = new Vector2((Single)((r.NextDouble() * 100) - 50), (Single)((r.NextDouble() * 100) - 50));
                e.Rotation = (Single)((r.NextDouble() * 10) - 5);
            }

            this.Group.Users.Added += this.HandleUserAdded;
        }
        #endregion

        #region Frame Methods
        protected override void update(GameTime gameTime)
        {
            base.update(gameTime);

            this.entities.Update(gameTime);
        }
        #endregion

        #region Event Handlers
        private void HandleUserAdded(object sender, User user)
        {
            using (FileStream input = File.OpenRead("./ship-part-export.dat"))
            {
                // var bridge = _shipBuilder.Import(input);

                var bridge = this.entities.Create<ShipPart>("entity:ship-part:hull:square");
                var ship = _ships.GetOrCreateAvailableShip();
                var player = this.entities.Create<Player>("entity:player:user", user);

                player.TrySetShip(ship);
                ship.TrySetBridge(bridge);

                var om = this.Group.CreateMessage("chat", NetDeliveryMethod.ReliableOrdered, 2);

                om.Write(false);
                om.Write($"Welcome, {user.Get("name")}.");
            }
        }
        #endregion

        #region Message Handlers
        private void HandleChatMessage(NetIncomingMessage obj)
        {
            // Parse the message then broadcast to all users
            var user = this.Users.GetByNetConnection(obj.SenderConnection);
            var om = this.Group.CreateMessage("chat", NetDeliveryMethod.ReliableOrdered, 2);

            om.Write(true);
            om.Write(user.Id);
            om.Write(obj.ReadString());
        }
        #endregion
    }
}
