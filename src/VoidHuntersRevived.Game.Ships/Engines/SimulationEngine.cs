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
        private readonly IPieceService _pieces;
        private readonly IBlueprintService _blueprints;

        public SimulationEngine(ITreeService treeFactory, IPieceService pieces, IBlueprintService blueprints)
        {
            _trees = treeFactory;
            _pieces = pieces;
            _blueprints = blueprints;
        }

        public void Process(VhId eventId, Simulation_Begin data)
        {
            //_trees.Spawn(eventId.Create(1), Teams.TeamZero, EntityTypes.Chain, _pieces.All<ThrusterDescriptor>().First().EntityType, null);
            //_trees.Spawn(eventId.Create(int.MaxValue), Teams.TeamZero, EntityTypes.Chain, _blueprints.GetAll().First(), null);
            //for(int j=0; j<1; j++)
            //{
            //    _trees.Spawn(eventId.Create(int.MaxValue), Teams.TeamZero, EntityTypes.Chain, EntityTypes.Pieces.HullTriangle);
            //}

            int i = 0;
            int radius = 2;
            int step = 2;
            FixVector2 offset = new FixVector2(0, 0);
            for (int x = -radius; x < radius; x += step)
            {
                for (int y = -radius; y < radius; y += step)
                {
                    _trees.Spawn(eventId, eventId.Create(i++), Teams.TeamZero, EntityTypes.Chain, _pieces.All()[i % _pieces.All().Length].EntityType, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                    {
                        initializer.Init(new Location()
                        {
                            Position = offset + new FixVector2((Fix64)x, (Fix64)y)
                        });
                    });
                }

            }

            //_trees.Spawn(eventId.Create(2), EntityTypes.UserShip, EntityTypes.Pieces.HullSquare);
        }
    }
}
