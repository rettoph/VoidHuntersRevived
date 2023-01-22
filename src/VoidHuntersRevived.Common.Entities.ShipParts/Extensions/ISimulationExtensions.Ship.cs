using MonoGame.Extended.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Extensions;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static partial class ISimulationExtensions
    {
        public static Entity CreateShip(this ISimulation simulation, ParallelKey key, string bridgeConfiguration)
        {
            return simulation.CreateEntity(key).MakeShip(
                aether: simulation.Aether,
                bridge: simulation.CreateShipPart(key.Create(ParallelTypes.ShipPart), bridgeConfiguration));
        }
    }
}
