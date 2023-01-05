using MonoGame.Extended.Entities;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static partial class ISimulationExtensions
    {
        public static Entity CreateShip(this ISimulation simulation, ParallelKey key)
        {
            var ship = simulation.CreateRootable(key);
            ship.Attach(new Pilotable());

            return ship;
        }
    }
}
