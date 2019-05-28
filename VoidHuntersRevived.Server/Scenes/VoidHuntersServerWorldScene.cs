using System;
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
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Server.Scenes
{
    class VoidHuntersServerWorldScene : VoidHuntersWorldScene
    {
        protected ServerPeer server;

        public VoidHuntersServerWorldScene(ServerPeer server, World world, IServiceProvider provider) : base(server, world, provider)
        {
            this.server = server;
        }

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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.entities.Update(gameTime);
        }

        #region Event Handlers
        private void HandleUserAdded(object sender, User user)
        {
            var bridge = this.entities.Create<ShipPart>("entity:ship-part");

            var player = this.entities.Create<Player>("entity:player");
            player.SetUser(user);
            player.SetBridge(bridge);
        }
        #endregion
    }
}
