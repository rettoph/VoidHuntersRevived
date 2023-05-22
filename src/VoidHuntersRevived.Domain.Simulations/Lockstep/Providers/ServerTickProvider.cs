using Guppy.Common;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Lockstep.Factories;
using VoidHuntersRevived.Domain.Simulations.Factories;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Providers
{
    [PeerTypeFilter(PeerType.Server)]
    internal sealed class ServerTickProvider : DefaultTickProvider
    {
        private readonly ITickFactory _factory;

        public ServerTickProvider(IFiltered<ITickFactory> factory)
        {
            _factory = factory.Instance;
        }

        public override bool TryDequeueNext([MaybeNullWhen(false)] out Tick tick)
        {
            if(base.TryDequeueNext(out tick))
            {
                return true;
            }

            tick = _factory.Create(this.nextId++);
            return true;
        }
    }
}
