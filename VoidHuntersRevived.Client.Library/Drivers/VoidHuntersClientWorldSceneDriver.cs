using Guppy.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    class VoidHuntersClientWorldSceneDriver : Driver
    {
        private VoidHuntersClientWorldScene _scene;

        public VoidHuntersClientWorldSceneDriver(VoidHuntersClientWorldScene scene, IServiceProvider provider, ILogger logger) : base(scene, provider, logger)
        {
            _scene = scene;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _scene.Server.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
        }
    }
}
