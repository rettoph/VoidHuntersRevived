using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library
{
    /// <summary>
    /// Represents the core game class.
    /// 
    /// ServerGame and ClientGame will extend this in
    /// peer specific libraries.
    /// </summary>
    public class BaseGame : Guppy.Game
    {
        #region Private Fields
        private Peer _peer;
        #endregion

        #region Constructor
        public BaseGame(Peer peer)
        {
            _peer = peer;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _peer.TryUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _peer.TryDraw(gameTime);
        }
        #endregion
    }
}
