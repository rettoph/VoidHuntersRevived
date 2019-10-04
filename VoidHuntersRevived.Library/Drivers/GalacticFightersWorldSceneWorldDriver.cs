using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Scenes;
using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Drivers
{
    /// <summary>
    /// Driver that will automatically step the world forward every frame
    /// </summary>
    [IsDriver(typeof(GalacticFightersWorldScene), 150)]
    public class GalacticFightersWorldSceneWorldDriver : Driver<GalacticFightersWorldScene>
    {
        private World _world;

        public GalacticFightersWorldSceneWorldDriver(World world, GalacticFightersWorldScene driven) : base(driven)
        {
            _world = world;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
        }
    }
}
