using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static class ISimulationExtensions
    {
        public static int CreateShip(this ISimulation simulation, ParallelKey key, ShipPart bridge)
        {
            return simulation.CreateEntity(key, EntityTypes.Ship, entity =>
            {
                IBody body = entity.Get<ISimulation>().Space.Create(key);
                body.SetTransform(FixVector2.Zero, Fix64.Zero);

                entity.Attach(body);
                bridge.Clone().AttachTo(entity);
            });
        }

        public static int CreateShipPart(
            this ISimulation simulation, 
            ParallelKey key, 
            ShipPart shipPart, 
            bool tractorable,
            FixVector2 position,
            Fix64 rotation)
        {
            return simulation.CreateEntity(key, EntityTypes.ShipPart, entity =>
            {
                IBody body = entity.Get<ISimulation>().Space.Create(key);
                body.SetTransform(position, rotation);

                entity.Attach(body);
                shipPart.Clone().AttachTo(entity);

                if(tractorable)
                {
                    entity.Attach(new Tractorable());
                }
            });
        }
    }
}
