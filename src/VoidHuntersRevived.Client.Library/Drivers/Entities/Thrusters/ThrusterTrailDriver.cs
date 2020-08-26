using Guppy;
using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.Thrusters
{
    internal sealed class ThrusterTrailDriver : Driver<Thruster>
    {
        #region Private Fields
        private Trail _trail;
        #endregion

        #region Lifecycle Methods
        protected override void Configure(object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            _trail = provider.GetService<TrailManager>().SetupTrail(this.driven);

            this.driven.OnUpdate += _trail.Update;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.driven.OnUpdate -= _trail.Update;
            _trail.TryDispose();
        }
        #endregion
    }
}
