using MonoGame.Extended.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Extensions;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static partial class ISimulationExtensions
    {
        public static Entity CreateShip(this ISimulation simulation, ParallelKey key, string bridgeConfiguration)
        {
            simulation.PublishEvent(new CreateShip(key, bridgeConfiguration));

            return simulation.GetEntity(key);
        }
    }
}
