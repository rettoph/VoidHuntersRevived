using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Resources.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS.Factories;
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerTypeFilter(PeerType.Server)]
    internal sealed class LockstepSimulation_Server : LockstepSimulation
    {
        private List<EventDto> _events = new List<EventDto>();

        public override void Enqueue(EventDto data)
        {
            _events.Add(data);
        }

        public LockstepSimulation_Server(ISettingProvider settings, IWorldFactory worldFactory, ISpaceFactory spaceFactory) : base(settings, worldFactory, spaceFactory)
        {
        }

        public override void Publish(EventDto data)
        {
            throw new NotImplementedException();
        }
    }
}
