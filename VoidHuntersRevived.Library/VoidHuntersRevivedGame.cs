using VoidHuntersRevived.Library.Scenes;
using Guppy;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library
{
    public class VoidHuntersRevivedGame : Guppy.Game
    {
        private Peer _peer;

        public VoidHuntersRevivedGame(Peer peer)
        {
            _peer = peer;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _peer.TryDraw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _peer.TryUpdate(gameTime);
        }
    }
}
