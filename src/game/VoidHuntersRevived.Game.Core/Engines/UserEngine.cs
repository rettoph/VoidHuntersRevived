using Guppy.Core.Network.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Game.Core.Engines
{
    internal sealed class UserEngine(
        ITreeService treeService,
        ITeamService teamService,
        IEntityTemplateFragmentService entityTemplateService,
        IBlueprintService blueprintService,
        INetScope<IStrategy> scope) : StrategyEngine, IGetReadyEngine,
        IEventEngine<UserJoined>
    {
        private readonly INetScope<IStrategy> _scope = scope;
        private readonly ITreeService _treeService = treeService;
        private readonly ITeamService _teamService = teamService;
        private readonly IEntityTemplateFragmentService _entityTemplateService = entityTemplateService;
        private readonly IBlueprintService _blueprintService = blueprintService;

        public string name { get; } = nameof(UserEngine);

        public void Process(VhId eventId, UserJoined data)
        {
            // IEntityTemplate<HullEntityTemplate> hull = _entityTemplateService.GetAll<HullEntityTemplate>().Last();

            //_trees.Spawn(shipId, Teams.TeamOne, EntityTemplates.UserShip, hull.EntityTemplate);
            // _treeFactory.Create(id.Create(1), EntityTemplates.Chain, PieceTypes.HullSquare);

            Blueprint blueprint = _blueprintService.GetAll().First();
            _treeService.Spawn(eventId, eventId.Create(1), _teamService.GetOpenTeam(), Resources.EntityTemplates.Ship.UserShipEntityTemplate, blueprint, (IEntityService entities, IEntityTemplate entityTemplate, EntityId id, ref EntityInitializer initializer) =>
            {
                initializer.Init(new Location()
                {
                    Position = new FixVector2(0, 0)
                });
                initializer.Init(new UserId(data.UserDto.Id));
            });
        }
    }
}
