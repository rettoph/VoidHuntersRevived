using Guppy.Attributes;
using Guppy.Network;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Events;

namespace VoidHuntersRevived.Game.Core.Engines
{
    [AutoLoad]
    internal sealed class UserEngine : BasicEngine, IGetReadyEngine,
        IEventEngine<UserJoined>
    {
        private readonly INetGroup _scope;
        private readonly ITreeService _trees;
        private readonly IPieceService _pieces;
        private readonly IBlueprintService _blueprints;

        public UserEngine(ITreeService trees, IPieceService pieces, IBlueprintService blueprints, INetGroup scope)
        {
            _scope = scope;
            _trees = trees;
            _pieces = pieces;
            _blueprints = blueprints;
        }

        public string name { get; } = nameof(UserEngine);

        public void Process(VhId eventId, UserJoined data)
        {
            var hull = _pieces.All<HullDescriptor>().Last();

            //_trees.Spawn(shipId, Teams.TeamOne, EntityTypes.UserShip, hull.EntityType);
            // _treeFactory.Create(id.Create(1), EntityTypes.Chain, PieceTypes.HullSquare);

            var blueprint = _blueprints.GetAll().First();
            _trees.Spawn(eventId, eventId.Create(1), Teams.TeamOne, EntityTypes.UserShip, blueprint, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
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
