using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Game.Core.Engines
{
    [AutoLoad]
    internal sealed class UserEngine : StrategyEngine, IGetReadyEngine,
        IEventEngine<UserJoined>
    {
        private readonly INetScope<IStrategy> _scope;
        private readonly ITreeService _treeService;
        private readonly ITeamService _teamService;
        private readonly IEntityTypeService _entityTypeService;
        private readonly IBlueprintService _blueprintService;

        public UserEngine(
            ITreeService treeService,
            ITeamService teamService,
            IEntityTypeService entityTypeService,
            IBlueprintService blueprintService,
            INetScope<IStrategy> scope)
        {
            _scope = scope;
            _treeService = treeService;
            _teamService = teamService;
            _entityTypeService = entityTypeService;
            _blueprintService = blueprintService;
        }

        public string name { get; } = nameof(UserEngine);

        public void Process(VhId eventId, UserJoined data)
        {
            var hull = _entityTypeService.GetAll<HullDescriptor>().Last();

            //_trees.Spawn(shipId, Teams.TeamOne, EntityTypes.UserShip, hull.EntityType);
            // _treeFactory.Create(id.Create(1), EntityTypes.Chain, PieceTypes.HullSquare);

            var blueprint = _blueprintService.GetAll().First();
            _treeService.Spawn(eventId, eventId.Create(1), _teamService.GetOpenTeamComponent(), EntityTypes.UserShip, blueprint, (IEntityService entities, IEntityType type, EntityId id, ref EntityInitializer initializer) =>
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
