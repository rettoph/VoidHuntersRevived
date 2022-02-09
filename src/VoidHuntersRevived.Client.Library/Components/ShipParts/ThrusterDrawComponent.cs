using Guppy.EntityComponent.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Client.Library.Graphics.Services;
using VoidHuntersRevived.Client.Library.Graphics.Utilities;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Client.Library.Components.ShipParts
{
    internal sealed class ThrusterDrawComponent : DrawComponent<Thruster>
    {
        private TrailService _trails;
        private Trail _trail;

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _trails);

            this.Entity.OnPoweredChanged += this.HandlePowerChanged;
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.OnPoweredChanged -= this.HandlePowerChanged;

            _trail?.Deactivate();
            _trail = default;
        }

        private void HandlePowerChanged(Thruster sender, bool value)
        {
            _trail?.Deactivate();
            _trail = value ? _trails.Create(this.Entity) : null;
        }
    }
}
