using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Server.Scenes
{
    class VoidHuntersServerWorldScene : VoidHuntersWorldScene
    {
        protected ServerPeer server;

        public VoidHuntersServerWorldScene(ServerPeer peer, World world, IServiceProvider provider) : base(peer, world, provider)
        {
            this.server = server;
        }
    }
}
