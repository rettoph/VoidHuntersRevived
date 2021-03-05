using Guppy;
using Guppy.DependencyInjection;
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
    internal sealed class ThrusterTrailsDriver : Driver<Thruster>
    {
        #region Lifecycle Methods
        protected override void Initialize(Thruster driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);
        }

        protected override void Release(Thruster driven)
        {
            base.Release(driven);
        }
        #endregion
    }
}
