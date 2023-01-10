using MonoGame.Extended.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities.Extensions
{
    public static partial class ISimulationExtensions
    {
        public static Entity CreateShip(this ISimulation simulation, ParallelKey key, string bridgeConfiguration)
        {
            return simulation.CreateEntity(key).MakeShip(
                aether: simulation.Aether,
                bridge: simulation.CreateShipPart(ParallelKey.From(ParallelTypes.ShipPart, key), bridgeConfiguration));
        }
    }
}
