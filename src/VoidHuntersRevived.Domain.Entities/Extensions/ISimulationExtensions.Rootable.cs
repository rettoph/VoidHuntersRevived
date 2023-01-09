using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities.Extensions
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
