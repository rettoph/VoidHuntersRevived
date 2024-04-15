using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Game.Core.Engines
{
    [AutoLoad]
    internal sealed class UserEngine : BasicEngine, IGetReadyEngine,
        IEventEngine<UserJoined>
    {
        private readonly INetGroup _scope;
        private readonly ITreeService _trees;
        private readonly ITeamService _teams;
        private readonly IEntityTypeService _entityTypes;
        private readonly IBlueprintService _blueprints;

        public UserEngine(ITreeService trees, ITeamService teams, IEntityTypeService entityTypes, IBlueprintService blueprints, INetGroup scope)
        {
            _scope = scope;
            _trees = trees;
            _teams = teams;
            _entityTypes = entityTypes;
            _blueprints = blueprints;
        }

        public string name { get; } = nameof(UserEngine);

        public void Process(VhId eventId, UserJoined data)
        {
            var hull = _entityTypes.GetAll<HullDescriptor>().Last();

            //_trees.Spawn(shipId, Teams.TeamOne, EntityTypes.UserShip, hull.EntityType);
            // _treeFactory.Create(id.Create(1), EntityTypes.Chain, PieceTypes.HullSquare);

            var blueprint = _blueprints.GetAll().First();
            _trees.Spawn(eventId, eventId.Create(1), _teams.GetOpenTeamId(), EntityTypes.UserShip, blueprint, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
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
