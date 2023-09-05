using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Identity;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
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

        public UserEngine(ITreeService trees, NetScope scope)
        {
            _scope = scope;
            _trees = trees;
        }

        public string name { get; } = nameof(UserEngine);

        public void Process(VhId id, UserJoined data)
        {
            VhId shipId = _scope.Peer!.Users.UpdateOrCreate(data.UserId, data.Claims).GetUserShipId();

            _trees.Spawn(shipId, TeamId.Blue, EntityTypes.UserShip, EntityTypes.Pieces.HullSquare);
            // _treeFactory.Create(id.Create(1), EntityTypes.Chain, PieceTypes.HullSquare);
        }
    }
}
