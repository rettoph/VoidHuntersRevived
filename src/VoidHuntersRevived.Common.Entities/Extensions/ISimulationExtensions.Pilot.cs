using MonoGame.Extended.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static partial class ISimulationExtensions
    {
        public static Entity CreatePilot(this ISimulation simulation, ParallelKey key, Entity pilotable)
        {
            return simulation.CreateEntity(key).MakePilot(pilotable);
        }
    }
}
