﻿using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Lockstep.Factories;

namespace VoidHuntersRevived.Domain.Simulations.Factories
{
    [PeerTypeFilter(PeerType.Client)]
    internal sealed class ClientTickFactory : ITickFactory
    {
        public void Enqueue(EventDto input)
        {
            throw new NotImplementedException();
        }

        public Tick Create(int id)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
