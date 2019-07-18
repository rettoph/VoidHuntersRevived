﻿using System;
using System.Collections.Generic;
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
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Server.Scenes
{
    class VoidHuntersServerWorldScene : VoidHuntersWorldScene
    {
        private ShipCollection _ships;

        protected ServerPeer server;

        public VoidHuntersServerWorldScene(ShipCollection ships, ServerPeer server, World world, IServiceProvider provider) : base(server, world, provider)
        {
            _ships = ships;

            this.server = server;
        }

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();
        }

        protected override void Initialize()
        {
            base.Initialize();

            var r = new Random();

            for(Int32 i=0; i<100; i++)
            {
                var e = this.entities.Create<ShipPart>("entity:ship-part");
                e.Position = new Vector2((Single)((r.NextDouble() * 100) - 50), (Single)((r.NextDouble() * 100) - 50));
                e.Rotation = (Single)((r.NextDouble() * 10) - 5);
            }

            this.Group.Users.Added += this.HandleUserAdded;
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            // this.Group.MessageHandler.Add("chat", this.HandleChatMessage);
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
            var bridge = this.entities.Create<ShipPart>("entity:ship-part");
            var ship = _ships.GetOrCreateAvailableShip();
            var player = this.entities.Create<Player>("entity:player:user", user);

            player.TrySetShip(ship);
            ship.TrySetBridge(bridge);

            var om = this.Group.CreateMessage("chat", NetDeliveryMethod.ReliableOrdered, 2);

            om.Write(false);
            om.Write($"Welcome, {user.Get("name")}.");
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
