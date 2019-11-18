using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Drivers.Scenes
{
    /// <summary>
    /// Driver specifically used to update the Farseer 
    /// world each frame.
    /// </summary>
    [IsDriver(typeof(WorldScene), 110)]
    public sealed class WorldWorldSceneDriver : Driver<WorldScene>
    {
        #region Private Attributes
        private World _world;
        #endregion

        #region Constructor
        public WorldWorldSceneDriver(World world, WorldScene driven) : base(driven)
        {
            _world = world;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _world.Step((Single)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
        }
        #endregion
    }
}
