using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Game.Common;

namespace VoidHuntersRevived.Game.Ships.Engines
{
    [AutoLoad]
    internal sealed class SimulationEngine : BasicEngine, IEventEngine<Simulation_Begin>
    {
        private readonly ITreeService _trees;

        public SimulationEngine(ITreeService treeFactory)
        {
            _trees = treeFactory;
        }

        public void Process(VhId eventId, Simulation_Begin data)
        {
            for(int i=0; i<1000; i++)
            {
                _trees.Spawn(eventId.Create(i), TeamId.Default, EntityTypes.Chain, EntityTypes.Pieces.HullTriangle);
            }

            //for (int x = -30; x < 30; x++)
            //{
            //    for (int y = -30; y < 30; y++)
            //    {
            //        _trees.Spawn(eventId.Create(i++), TeamId.Default, EntityTypes.Chain, EntityTypes.Pieces.HullTriangle, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            //        {
            //            initializer.Init(new Location()
            //            {
            //                Position = new FixVector2((Fix64)x, (Fix64)y)
            //            });
            //        });
            //    }
            //
            //}

            //_trees.Spawn(eventId.Create(2), EntityTypes.UserShip, EntityTypes.Pieces.HullSquare);
        }
    }
}
