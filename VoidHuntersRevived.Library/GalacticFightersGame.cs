using GalacticFighters.Library.Scenes;
using Guppy;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library
{
    public class GalacticFightersGame : Guppy.Game
    {
        private Peer _peer;

        public GalacticFightersGame(Peer peer)
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
