using Guppy;
using Guppy.DependencyInjection;
using System;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.Thrusters
{
    /// <summary>
    /// Custom driver to manage the creation &
    /// rendering of all internal thruster trails.
    /// </summary>
    internal sealed class ThrusterGraphicsDriver : Driver<Thruster>
    {
        #region Private Fields
        private TrailService _trails;
        private Trail _trail;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(Thruster driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _trails);

            this.driven.OnImpulse += this.HandleDrivenImpulse;
            this.driven.OnChainChanged += this.HandleDrivenChainChanged;
        }

        protected override void Release(Thruster driven)
        {
            base.Release(driven);

            _trail = null;

            this.driven.OnImpulse -= this.HandleDrivenImpulse;
            this.driven.OnChainChanged -= this.HandleDrivenChainChanged;
        }
        #endregion

        #region Helper Methods
        private void TryClearTrail()
        {
            if (_trail != default)
            { // Reset trail if possible
                _trail.Thruster = null;
                _trail = null;
            }
        }
        #endregion

        #region Event Handlers
        private void HandleDrivenImpulse(Thruster sender, bool value)
        {
            if (value)
                _trail = _trails.Create(this.driven);
            else
                this.TryClearTrail();
        }

        private void HandleDrivenChainChanged(ShipPart sender, Chain old, Chain value)
            => this.TryClearTrail();
        #endregion
    }
}
