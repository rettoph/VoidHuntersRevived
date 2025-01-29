using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Events;

namespace VoidHuntersRevived.Game.Core.Engines
{
    public sealed class SimulationEngine(
    //ITreeService treeService,
    //ITeamService teamService,
    //IEntityTemplateFragmentService entityTemplateService,
    //IBlueprintService blueprintService
    ) : StrategyEngine, IEventEngine<Simulation_Begin>
    {
        //private readonly ITreeService _treeService = treeService;
        //private readonly ITeamService _teamService = teamService;
        //private readonly IEntityTemplateFragmentService _entityTemplateService = entityTemplateService;
        //private readonly IBlueprintService _blueprintService = blueprintService;

        public void Process(VhId eventId, Simulation_Begin data)
        {
            //_trees.Spawn(eventId.Create(1), Teams.TeamZero, EntityTemplates.Chain, _pieces.All<ThrusterDescriptor>().First().EntityTemplate, null);
            //_trees.Spawn(eventId.Create(int.MaxValue), Teams.TeamZero, EntityTemplates.Chain, _blueprints.GetAll().First(), null);
            //for(int j=0; j<1; j++)
            //{
            //    _trees.Spawn(eventId.Create(int.MaxValue), Teams.TeamZero, EntityTemplates.Chain, EntityTemplates.Pieces.HullTriangle);
            //}

            // int i = 0;
            // int radius = 2;
            // int step = 2;
            // FixVector2 offset = new(0, 0);
            // PieceEntityTemplate[] pieceTypes = _entityTemplateService.GetAll<PieceEntityTemplate>();
            // for (int x = -radius; x < radius; x += step)
            // {
            //     for (int y = -radius; y < radius; y += step)
            //     {
            //         _treeService.Spawn(eventId, eventId.Create(i++), _teamService.GetDefaultTeamComponent(), ChainEntityTemplate.ChainEntityTemplateKey, pieceTypes[i % pieceTypes.Length].Key, (IEntityService entities, EntityTemplate entityTemplate, in Entity entity, ref EntityInitializer initializer) =>
            //         {
            //             initializer.Init(new Location()
            //             {
            //                 Position = offset + new FixVector2((Fix64)x, (Fix64)y)
            //             });
            //         });
            //     }
            // }

            //_trees.Spawn(eventId.Create(2), EntityTemplates.UserShip, EntityTemplates.Pieces.HullSquare);
        }
    }
}