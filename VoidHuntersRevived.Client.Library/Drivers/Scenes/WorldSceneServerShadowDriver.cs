using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Drivers.Scenes
{
    /// <summary>
    /// Simple driver used to update the client's server shadow
    /// world each frame.
    /// </summary>
    [IsDriver(typeof(WorldScene), 110)]
    internal sealed class WorldSceneServerShadowDriver : Driver<WorldScene>
    {
        #region Private Fields
        private ServerShadow _shadow;
        #endregion

        #region Constructor
        public WorldSceneServerShadowDriver(ServerShadow shadow, WorldScene driven) : base(driven)
        {
            _shadow = shadow;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _shadow.World.Step((Single)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
        }
        #endregion
    }
}
