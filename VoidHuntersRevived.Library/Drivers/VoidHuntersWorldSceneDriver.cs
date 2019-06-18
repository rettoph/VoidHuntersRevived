using FarseerPhysics.Dynamics;
using Guppy.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Drivers
{
    public class VoidHuntersWorldSceneDriver : Driver
    {
        private World _world;

        public VoidHuntersWorldSceneDriver(World world, VoidHuntersWorldScene scene, IServiceProvider provider) : base(scene, provider)
        {
            _world = world;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
        }
    }
}
