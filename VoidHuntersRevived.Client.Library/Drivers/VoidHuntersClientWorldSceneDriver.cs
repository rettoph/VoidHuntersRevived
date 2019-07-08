using Guppy.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Client.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    class VoidHuntersClientWorldSceneDriver : Driver
    {
        private ServerRender _server;

        public VoidHuntersClientWorldSceneDriver(ServerRender server, VoidHuntersClientWorldScene scene, IServiceProvider provider) : base(scene, provider)
        {
            _server = server;
        }

        #region Frame Methods
        protected override void update(GameTime gameTime)
        {
            base.update(gameTime);

            _server.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
        }
        #endregion
    }
}
