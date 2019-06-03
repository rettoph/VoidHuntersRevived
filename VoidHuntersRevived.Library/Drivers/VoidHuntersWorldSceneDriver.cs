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
        private VoidHuntersWorldScene _scene;

        public VoidHuntersWorldSceneDriver(VoidHuntersWorldScene scene, IServiceProvider provider, ILogger logger) : base(scene, provider, logger)
        {
            _scene = scene;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _scene.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
        }
    }
}
