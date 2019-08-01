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
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.Loaders;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Server.Drivers;

namespace VoidHuntersRevived.Server.Scenes
{
    class VoidHuntersServerWorldScene : VoidHuntersWorldScene
    {
        private ShipBuilder _shipBuilder;
        private RandomTypeLoader _randomTypeLoader;

        protected ServerPeer server;

        public VoidHuntersServerWorldScene(ShipBuilder shipBuilder, RandomTypeLoader entityTypeLoader, ServerPeer server, World world, IServiceProvider provider) : base(server, world, provider)
        {
            _shipBuilder = shipBuilder;
            _randomTypeLoader = entityTypeLoader;

            this.server = server;
        }

        #region Initialization Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.Group.Messages.AddHandler("chat", this.HandleChatMessage);
        }

        protected override void Initialize()
        {
            base.Initialize();

            var r = new Random();

            for(Int32 i=0; i<100; i++)
            {
                var e = this.entities.Create<ShipPart>(_randomTypeLoader.GetRandomValue("ship-part:hull", r));
                e.Position = new Vector2((Single)((r.NextDouble() * 150) - 75), (Single)((r.NextDouble() * 150) - 75));
                e.Rotation = (Single)((r.NextDouble() * 10) - 5);

                e = this.entities.Create<ShipPart>(_randomTypeLoader.GetRandomValue("ship-part:thruster", r));
                e.Position = new Vector2((Single)((r.NextDouble() * 150) - 75), (Single)((r.NextDouble() * 150) - 75));
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
            ShipPart bridge;
            var r = new Random();
            // using (FileStream input = File.OpenRead(_randomTypeLoader.GetRandomValue("ship-part-export", r)))
            //     bridge = _shipBuilder.Import(input);

            bridge = this.entities.Create<ShipPart>(_randomTypeLoader.GetRandomValue("ship-part:bridge", r));
            bridge.SetTransform(
                new Vector2((Single)((r.NextDouble() * 150) - 75), (Single)((r.NextDouble() * 150) - 75)),
                (Single)((r.NextDouble() * 10) - 5));

            var ship = this.entities.Create<Ship>("entity:ship");
            var player = this.entities.Create<Player>("entity:player:user", user);

            player.TrySetShip(ship);
            ship.TrySetBridge(bridge);

            var om = this.Group.CreateMessage("chat", NetDeliveryMethod.ReliableOrdered, 2);

            om.Write(false);
            om.Write($"Welcome, {user.Get("name")}.");
        }
        #endregion

        #region Message Handlers
        private void HandleChatMessage(Object sender, NetIncomingMessage obj)
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
