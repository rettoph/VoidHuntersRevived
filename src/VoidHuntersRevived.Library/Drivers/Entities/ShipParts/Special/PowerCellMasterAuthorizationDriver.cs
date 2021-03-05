using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipParts.Special
{
    internal sealed class PowerCellMasterAuthorizationDriver : MasterNetworkAuthorizationDriver<PowerCell>
    {
        #region Private Fields
        private WorldEntity _world;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(PowerCell driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _world);

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
            if (sender.Health <= 0)
            {
                _world.EnqeueExplosion(sender.WorldCenter, sender.Chain.Ship?.Color ?? sender.Color, 15f, 250f, 100f);
                sender.TryRelease();
            }
                
        }
        #endregion
    }
}
