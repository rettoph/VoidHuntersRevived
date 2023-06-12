using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Providers;
using Guppy.Network.Peers;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Common.Components;

namespace VoidHuntersRevived.Game.Systems
{
    [AutoLoad]
    internal sealed class UserSystem : BasicSystem,
        IEventSubscriber<UserJoined>,
        IStepSystem<Helm>
    {
        private readonly NetScope _scope;

        public UserSystem(NetScope scope)
        {
            _scope = scope;
        }

        public void Process(Guid id, UserJoined data)
        {
            User user = _scope.Peer!.Users.UpdateOrCreate(data.UserId, data.Claims);

            this.Simulation.Entities.Create(EntityTypes.Pilot, user.GetPilotId(), initializer =>
            {
                initializer.Set(new Pilot()
                {
                    ShipId = this.Simulation.Entities.Create(EntityTypes.Ship, id.Create(1))
                });
            });
        }

        public void Step(Step step, in Guid id, ref Helm component1)
        {
        }
    }
}
