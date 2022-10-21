using Guppy;
using Guppy.MonoGame;
using Guppy.MonoGame.Services;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library
{
    public abstract class MainGuppy : FrameableGuppy
    {
        private Peer _peer;

        protected MainGuppy(
            Peer peer,
            World world) : base(world)
        {
            _peer = peer;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _peer.Flush();
        }
    }
}
