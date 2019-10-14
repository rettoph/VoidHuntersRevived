using FarseerPhysics.Dynamics;
using VoidHuntersRevived.Library.Scenes;
using Guppy;
using Guppy.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace VoidHuntersRevived.Library.Drivers
{
    /// <summary>
    /// Driver that will automatically step the world forward every frame
    /// </summary>
    [IsDriver(typeof(WorldScene), 150)]
    public class WorldSceneWorldDriver : Driver<WorldScene>
    {
        private World _world;

        public WorldSceneWorldDriver(World world, WorldScene driven) : base(driven)
        {
            _world = world;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var sw = new Stopwatch();
            sw.Start();

            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
        }
    }
}
