using FarseerPhysics.Dynamics;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using GalacticFighters.Library.Scenes;
using Guppy;
using Guppy.Attributes;

namespace GalacticFighters.Library.Drivers
{
    [IsDriver(typeof(GalacticFightersWorldScene))]
    public class GalacticFightersWorldSceneDriver : Driver<GalacticFightersWorldScene>
    {
        private World _world;

        public GalacticFightersWorldSceneDriver(World world, GalacticFightersWorldScene scene) : base(scene)
        {
            _world = world;
        }

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
        }
        #endregion
    }
}
