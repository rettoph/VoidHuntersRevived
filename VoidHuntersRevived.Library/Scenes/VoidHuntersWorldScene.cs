using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Collections;
using Guppy.Network;
using Guppy.Network.Collections;
using Guppy.Network.Groups;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Scenes
{
    public class VoidHuntersWorldScene : NetworkScene
    {
        protected Peer peer;
        protected World world;

        public UserCollection Users { get { return this.group.Users; } }

        public VoidHuntersWorldScene(Peer peer, World world, IServiceProvider provider) : base(peer.Groups.GetOrCreateById(Guid.Empty), provider)
        {
            this.peer = peer;
            this.world = world;
        }

        public override void Update(GameTime gameTime)
        {
            this.world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);

            base.Update(gameTime);
        }
    }
}
