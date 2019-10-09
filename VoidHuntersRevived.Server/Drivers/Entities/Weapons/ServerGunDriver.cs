using GalacticFighters.Library.Entities.Ammo;
using GalacticFighters.Library.Entities.ShipParts.Weapons;
using Guppy;
using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Extensions.Lidgren;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System.Linq;
using Guppy.Extensions.Collection;
using Lidgren.Network;

namespace GalacticFighters.Server.Drivers.Entities.Weapons
{
    [IsDriver(typeof(Gun))]
    internal sealed class ServerGunDriver : Driver<Gun>
    {
        #region Static Attributes
        public static Single BulletUpdateRate { get; set; } = 150f;
        #endregion

        #region Private Fields
        private Double _lastBulletUpdate;
        #endregion

        #region Constructors
        public ServerGunDriver(Gun driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _lastBulletUpdate += gameTime.ElapsedGameTime.TotalMilliseconds;

            if(this.driven.Projectiles.Any() && _lastBulletUpdate >= ServerGunDriver.BulletUpdateRate)
            {
                // Send a message containing all bullet data for the current gun...
                var action = this.driven.Actions.Create("projectiles", NetDeliveryMethod.Unreliable, 10);
                action.Write(this.driven.FireCount);
                action.Write(this.driven.Projectiles.Count());
                this.driven.Projectiles.ForEach(b =>
                {
                    action.Write(b);
                    action.Write(b.Position);
                    action.Write(b.LinearVelocity);
                });

                _lastBulletUpdate %= ServerGunDriver.BulletUpdateRate;
            }
        }
        #endregion
    }
}
