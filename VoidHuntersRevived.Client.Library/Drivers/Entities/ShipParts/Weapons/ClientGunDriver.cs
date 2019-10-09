using GalacticFighters.Library.Entities.Ammo;
using GalacticFighters.Library.Entities.ShipParts.Weapons;
using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities.ShipParts.Weapons
{
    [IsDriver(typeof(Gun))]
    internal sealed class ClientGunDriver : Driver<Gun>
    {
        #region Private Fields
        private EntityCollection _entities;
        #endregion

        #region Constructors
        public ClientGunDriver(EntityCollection entities, Gun driven) : base(driven)
        {
            _entities = entities;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Actions.TryAdd("projectiles", this.HandleProjectilesAction);
        }
        #endregion

        #region Event Handlers
        private void HandleProjectilesAction(object sender, NetIncomingMessage arg)
        {
            var fireCount = arg.ReadUInt32();
            while (this.driven.FireCount < fireCount)
                this.driven.Fire(); // Force fire the gun, ensuring that is will catch up with the server

            var total = arg.ReadInt32();
            Projectile target;

            for (Int32 i=0; i<total; i++)
            { // Iterate through all bullets contained within the projectile bullet data
                if((target = arg.ReadEntity<Projectile>(_entities)) == default(Projectile))
                {
                    arg.Position += 16;
                }
                else
                {
                    target.ReadVitals(arg.ReadVector2(), arg.ReadVector2());
                }
            }
        }
        #endregion
    }
}
