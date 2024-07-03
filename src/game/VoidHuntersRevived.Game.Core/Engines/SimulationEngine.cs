using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Game.Core.Engines
{
    [AutoLoad]
    internal sealed class SimulationEngine : StrategyEngine, IEventEngine<Simulation_Begin>
    {
        private readonly ITreeService _trees;
        private readonly ITeamService _teams;
        private readonly IEntityTypeService _entityTypes;
        private readonly IBlueprintService _blueprints;

        public SimulationEngine(ITreeService treeFactory, ITeamService teams, IEntityTypeService entityTypes, IBlueprintService blueprints)
        {
            _trees = treeFactory;
            _teams = teams;
            _entityTypes = entityTypes;
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
            var pieces = _entityTypes.GetAll<PieceDescriptor>();
            for (int x = -radius; x < radius; x += step)
            {
                for (int y = -radius; y < radius; y += step)
                {
                    _trees.Spawn(eventId, eventId.Create(i++), _teams.GetDefaultTeamComponent(), EntityTypes.Chain, pieces[i % pieces.Length], (IEntityService entities, IEntityType type, EntityId id, ref EntityInitializer initializer) =>
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
