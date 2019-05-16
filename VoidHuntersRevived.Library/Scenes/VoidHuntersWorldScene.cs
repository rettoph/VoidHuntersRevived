using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Collections;
using Guppy.Network;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Players;

namespace VoidHuntersRevived.Library.Scenes
{
    public class VoidHuntersWorldScene : NetworkScene
    {
        protected Peer peer;
        protected World world;
        protected LivingObjectCollection<Player> players;

        public VoidHuntersWorldScene(Peer peer, World world, IServiceProvider provider) : base(peer.Groups.GetOrCreateById(Guid.Empty), provider)
        {
            this.peer = peer;
            this.world = world;
        }

        protected override void Boot()
        {
            base.Boot();

            this.players = new LivingObjectCollection<Player>(initializeOnAdd: true);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            this.world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);

            base.Update(gameTime);

            this.players.Update(gameTime);
        }
    }
}
