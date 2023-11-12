using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Identity;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Game.Common;

namespace VoidHuntersRevived.Game.Ships.Engines
{
    [AutoLoad]
    internal sealed class UserEngine : BasicEngine, IGetReadyEngine,
        IEventEngine<UserJoined>
    {
        private readonly NetScope _scope;
        private readonly ITreeService _trees;
        private readonly IPieceService _pieces;
        private readonly IBlueprintService _blueprints;

        public UserEngine(ITreeService trees, IPieceService pieces, IBlueprintService blueprints, NetScope scope)
        {
            _scope = scope;
            _trees = trees;
            _pieces = pieces;
            _blueprints = blueprints;
        }

        public string name { get; } = nameof(UserEngine);

        public void Process(VhId id, UserJoined data)
        {
            VhId shipId = _scope.Peer!.Users.UpdateOrCreate(data.UserId, data.Claims).GetUserShipId();

            var hull = _pieces.All<HullDescriptor>().Last();

            //_trees.Spawn(shipId, Teams.TeamOne, EntityTypes.UserShip, hull.EntityType);
            // _treeFactory.Create(id.Create(1), EntityTypes.Chain, PieceTypes.HullSquare);

            var blueprint = _blueprints.GetAll().First();
            _trees.Spawn(shipId, Teams.TeamOne, EntityTypes.UserShip, blueprint, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                initializer.Init(new Location()
                {
                    Position = new FixVector2(5, 5)
                });

            });
        }
    }
}
