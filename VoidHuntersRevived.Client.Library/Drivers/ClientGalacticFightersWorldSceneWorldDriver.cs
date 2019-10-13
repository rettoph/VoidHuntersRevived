using GalacticFighters.Client.Library.Scenes;
using GalacticFighters.Client.Library.Utilities;
using Guppy;
using Guppy.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers
{
    [IsDriver(typeof(ClientGalacticFightersWorldScene), 150)]
    public sealed class ClientGalacticFightersWorldSceneWorldDriver : Driver<ClientGalacticFightersWorldScene>
    {
        private ServerRender _server;

        public ClientGalacticFightersWorldSceneWorldDriver(ServerRender server, ClientGalacticFightersWorldScene driven) : base(driven)
        {
            _server = server;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _server.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
        }
    }
}
