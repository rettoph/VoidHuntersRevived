﻿using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Client.Library.Utilities;
using Guppy;
using Guppy.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    [IsDriver(typeof(ClientWorldScene), 150)]
    public sealed class ClientWorldSceneServerWorldDriver : Driver<ClientWorldScene>
    {
        private ServerRender _server;

        public ClientWorldSceneServerWorldDriver(ServerRender server, ClientWorldScene driven) : base(driven)
        {
            _server = server;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var sw = new Stopwatch();
            sw.Start();

            this.logger.LogInformation($"ClientWorldSceneServerWorldDriver Starting...");
            _server.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);

            sw.Stop();
            this.logger.LogInformation($"Done. {sw.ElapsedMilliseconds}");
        }
    }
}
