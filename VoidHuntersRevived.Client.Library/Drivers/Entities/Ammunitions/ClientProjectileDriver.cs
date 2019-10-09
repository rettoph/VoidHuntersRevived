using FarseerPhysics.Dynamics;
using GalacticFighters.Client.Library.Utilities;
using GalacticFighters.Library.Entities.Ammo;
using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities.Ammunitions
{
    [IsDriver(typeof(Projectile))]
    internal sealed class ClientProjectileDriver : Driver<Projectile>
    {
        private Body _serverBody;
        private ServerRender _server;

        #region Constructor
        public ClientProjectileDriver(ServerRender server, Projectile driven) : base(driven)
        {
            _server = server;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Clone the server body
            _serverBody = _server.CloneBody(this.driven.Body, true);
            this.driven.SetReadBody(_serverBody);
        }

        protected override void Dispose()
        {
            base.Dispose();

            _server.DestroyBody(this.driven.Body);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Lerp the client projectile towards the server data
            this.driven.Position = Vector2.Lerp(this.driven.Position, _serverBody.Position, 0.1f);
            this.driven.LinearVelocity = Vector2.Lerp(this.driven.LinearVelocity, _serverBody.LinearVelocity, 0.1f);
        }
        #endregion
    }
}
