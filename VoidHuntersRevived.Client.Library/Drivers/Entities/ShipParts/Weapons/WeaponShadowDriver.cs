using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using Guppy;
using Guppy.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;
using static VoidHuntersRevived.Library.Entities.ShipParts.ShipPart;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.Weapons
{
    /// <summary>
    /// Basic driver that will manage its weapon's
    /// shadow body.
    /// </summary>
    [IsDriver(typeof(Weapon))]
    internal sealed class WeaponShadowDriver : Driver<Weapon>
    {
        #region Private Fields
        private ServerShadow _server;
        private Body _shadow;
        private RevoluteJoint _joint;
        #endregion

        #region Constructors
        public WeaponShadowDriver(ServerShadow server, Weapon driven) : base(driven)
        {
            _server = server;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Cache the weapons server shadow...
            _shadow = _server[this.driven];

            // Setup the shadow...
            this.driven.AddFixtures(_shadow);
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            this.driven.OnChainUpdated += this.HandleChainUpdated;
        }

        public override void Dispose()
        {
            base.Dispose();

            this.driven.OnChainUpdated -= this.HandleChainUpdated;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.driven.Update(_server[this.driven.Root], _shadow, _joint);
        }
        #endregion

        #region Event Handlers
        private void HandleChainUpdated(object sender, ChainUpdate arg)
        {
            this.driven.UpdateJoint(_server[this.driven.Root], _shadow, _server.World, ref _joint);
        }
        #endregion
    }
}
