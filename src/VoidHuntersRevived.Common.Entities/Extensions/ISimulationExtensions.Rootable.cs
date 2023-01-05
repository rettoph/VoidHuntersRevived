using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using tainicom.Aether.Physics2D.Dynamics;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static partial class ISimulationExtensions
    {
        public static Entity CreateRootable(this ISimulation simulation, ParallelKey key)
        {
            var rootable = simulation.CreateEntity(key);
            rootable.Attach(new Rootable());
            rootable.Attach(simulation.Aether.CreateRectangle(1, 1, 1, Vector2.Zero, 0, BodyType.Dynamic));

            return rootable;
        }
    }
}
