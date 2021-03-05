using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipParts.Special
{
    internal sealed class PowerCellMasterAuthorizationDriver : MasterNetworkAuthorizationDriver<PowerCell>
    {
        #region Private Fields
        private ExplosionService _explosions;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(PowerCell driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _explosions);

            this.driven.OnHealthChanged += this.HandlePowerCellHealthChanged;
        }

        protected override void Release(PowerCell driven)
        {
            base.Release(driven);

            this.driven.OnHealthChanged -= this.HandlePowerCellHealthChanged;
        }
        #endregion

        #region Event Handlers
        private void HandlePowerCellHealthChanged(ShipPart sender, float old, float value)
        {
            if (sender.Health <= 0 && sender.Chain?.Ship != default)
            {
                _explosions.Create(new ExplosionContext()
                {
                    StartPosition = sender.Position,
                    StartVelocity = sender.LinearVelocity,
                    Color = Color.Lerp(sender.Chain.Ship.Color, sender.Context.DefaultColor, 0.5f),
                    MaxAge = 2f,
                    MaxDamagePerSecond = 100,
                    MaxForcePerSecond = 100,
                    MaxRadius = 25f
                });

                sender.TryRelease();
            }
                
        }
        #endregion
    }
}
