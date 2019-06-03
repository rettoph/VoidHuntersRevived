using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
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

        public World World { get; private set; }
        public UserCollection Users { get { return this.Group.Users; } }

        public VoidHuntersWorldScene(Peer peer, World world, IServiceProvider provider) : base(peer.Groups.GetOrCreateById(Guid.Empty), provider)
        {
            this.peer = peer;
            this.World = world;
        }
    }
}
