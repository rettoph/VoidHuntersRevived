using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
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
        public void Process(in EventId id, UserJoined data)
        {
            this.Simulation.Entities.Create(EntityTypes.Ship, id.EntityId());
        }

        public void Step(Step step, in EntityId id, ref Helm component1)
        {
        }
    }
}
