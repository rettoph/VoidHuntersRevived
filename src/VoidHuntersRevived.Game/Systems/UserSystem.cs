using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Providers;
using Guppy.Network.Peers;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Common.Components;
using VoidHuntersRevived.Game.Pieces;

namespace VoidHuntersRevived.Game.Systems
{
    [AutoLoad]
    internal sealed class UserSystem : BasicSystem,
        IEventSubscriber<UserJoined>,
        IStepSystem<Helm>
    {
        private readonly NetScope _scope;
        private readonly IPieceConfigurationService _pieces;

        public UserSystem(NetScope scope, IPieceConfigurationService pieces)
        {
            _scope = scope;
            _pieces = pieces;
        }

        public void Process(Guid id, UserJoined data)
        {
            User user = _scope.Peer!.Users.UpdateOrCreate(data.UserId, data.Claims);

            this.Simulation.Entities.Create(EntityTypes.UserShip, user.GetUserShipId(), initializer =>
            {
                initializer.Set(new UserOwned()
                {
                    UserId = data.UserId
                });
            });
        }

        public void Step(Step step, in Guid id, ref Helm component1)
        {
        }
    }
}
