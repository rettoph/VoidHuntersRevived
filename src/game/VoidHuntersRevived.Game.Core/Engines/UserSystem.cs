using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Game.Core.Systems
{
    public sealed class UserSystem(
        ITreeService treeService,
        ITeamService teamService,
        IBlueprintService blueprintService) : StrategySystem, IGetReadyEngine,
        IEventEngine<UserJoined>
    {
        private readonly ITreeService _treeService = treeService;
        private readonly ITeamService _teamService = teamService;
        private readonly IBlueprintService _blueprintService = blueprintService;

        public void Process(VhId eventId, UserJoined data)
        {
            // IEntityTemplate<HullEntityTemplate> hull = _entityTemplateService.GetAll<HullEntityTemplate>().Last();

            //_trees.Spawn(shipId, Teams.TeamOne, EntityTemplates.UserShip, hull.EntityTemplate);
            // _treeFactory.Create(id.Create(1), EntityTemplates.Chain, PieceTypes.HullSquare);

            Blueprint blueprint = this._blueprintService.GetAll().First();
            this._treeService.Spawn(eventId, eventId.Create(1).ToGlobalEntityId(), this._teamService.GetOpenTeam(), Resources.EntityTemplates.Ship.UserShipEntityTemplate, blueprint, (IEntityService entities, in InitializingEntity entity) =>
            {
                entity.Initializer.Init(new TractorBeamEmitter(entity.LocalId));
                entity.Initializer.Init(new UserId(data.UserDto.Id));
            });
        }
    }
}