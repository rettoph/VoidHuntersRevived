using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Game.Core.Engines
{
    [AutoLoad]
    internal sealed class SimulationEngine : StrategyEngine, IEventEngine<Simulation_Begin>
    {
        private readonly ITreeService _treeService;
        private readonly ITeamService _teamService;
        private readonly IEntityTypeService _entityTypeService;
        private readonly IBlueprintService _blueprintService;

        public SimulationEngine(
            ITreeService treeService,
            ITeamService teamService,
            IEntityTypeService entityTypeService,
            IBlueprintService blueprintService)
        {
            _treeService = treeService;
            _teamService = teamService;
            _entityTypeService = entityTypeService;
            _blueprintService = blueprintService;
        }

        public void Process(VhId eventId, Simulation_Begin data)
        {
            //_trees.Spawn(eventId.Create(1), Teams.TeamZero, EntityTypes.Chain, _pieces.All<ThrusterDescriptor>().First().EntityType, null);
            //_trees.Spawn(eventId.Create(int.MaxValue), Teams.TeamZero, EntityTypes.Chain, _blueprints.GetAll().First(), null);
            //for(int j=0; j<1; j++)
            //{
            //    _trees.Spawn(eventId.Create(int.MaxValue), Teams.TeamZero, EntityTypes.Chain, EntityTypes.Pieces.HullTriangle);
            //}

            // int i = 0;
            // int radius = 2;
            // int step = 2;
            // FixVector2 offset = new FixVector2(0, 0);
            // PieceEntityType[] pieceTypeKeys = _entityTypeService.GetAll<PieceEntityType>();
            // for (int x = -radius; x < radius; x += step)
            // {
            //     for (int y = -radius; y < radius; y += step)
            //     {
            //         _treeService.Spawn(eventId, eventId.Create(i++), _teamService.GetDefaultTeamComponent(), ChainEntityType.ChainEntityTypeKey, pieceTypeKeys[i % pieceTypeKeys.Length], (IEntityService entities, IEntityTypeProvider provider, EntityId id, ref EntityInitializer initializer) =>
            //         {
            //             initializer.Init(new Location()
            //             {
            //                 Position = offset + new FixVector2((Fix64)x, (Fix64)y)
            //             });
            //         });
            //     }
            // }

            //_trees.Spawn(eventId.Create(2), EntityTypes.UserShip, EntityTypes.Pieces.HullSquare);
        }
    }
}
