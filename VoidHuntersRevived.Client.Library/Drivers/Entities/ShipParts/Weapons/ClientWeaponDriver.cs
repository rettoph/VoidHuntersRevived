using FarseerPhysics.Dynamics;
using GalacticFighters.Client.Library.Utilities;
using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Entities.ShipParts.Weapons;
using Guppy;
using Guppy.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities.ShipParts.Weapons
{
    [IsDriver(typeof(Weapon))]
    internal sealed class ClientWeaponDriver : Driver<Weapon>
    {
        #region Private Fields
        private Body _serverBarrel;
        private ServerRender _server;
        #endregion

        #region Constructor
        public ClientWeaponDriver(ServerRender server, Weapon driven) : base(driven)
        {
            _server = server;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _serverBarrel = this.driven.Barrel.DeepClone(_server.World);

            this.driven.Events.TryAdd<ShipPart.ChainUpdate>("chain:updated", this.HandleChainUpdated);
        }

        protected override void Dispose()
        {
            base.Dispose();

            _serverBarrel.Dispose();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the client side chain recieves a chain update,
        /// we must ensure that the server render collision categories
        /// are updated as well
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleChainUpdated(object sender, ShipPart.ChainUpdate arg)
        {
            _serverBarrel.CollidesWith = this.driven.Root.CollidesWith;
            _serverBarrel.CollisionCategories = this.driven.Root.CollisionCategories;
        }
        #endregion
    }
}
