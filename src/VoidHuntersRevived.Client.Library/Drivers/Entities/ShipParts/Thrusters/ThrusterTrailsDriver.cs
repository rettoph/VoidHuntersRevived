﻿using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Interfaces;
using Guppy.IO.Commands.Services;
using Guppy.IO.Input;
using Guppy.IO.Services;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Extensions.Microsoft.Xna;
using VoidHuntersRevived.Library.Extensions.System;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.Thrusters
{
    /// <summary>
    /// Custom driver to manage the creation &
    /// rendering of all internal thruster trails.
    /// </summary>
    internal sealed class ThrusterTrailsDriver : Driver<Thruster>
    {
        #region Private Fields
        private TrailRenderService _trails;
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

            this.TryClearTrail();

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

        #region Event Methods
        private void HandleDrivenImpulse(Thruster sender, bool value)
        {
            if (value)
                _trail = _trails.BuildTrail(this.driven);
            else
                this.TryClearTrail();
                
        }

        private void HandleDrivenChainChanged(ShipPart sender, Chain old, Chain value)
            => this.TryClearTrail();
        #endregion
    }
}
