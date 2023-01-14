using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Factories
{
    [PeerTypeFilter(PeerType.Server)]
    internal sealed class ServerTickFactory : ITickFactory
    {
        private IList<IData> _datum;

        public ServerTickFactory()
        {
            _datum = new List<IData>();
        }

        public void Enqueue(IData data)
        {
            _datum.Add(data);
        }

        public Tick Create(int id)
        {
            if (_datum.Count == 0)
            {
                return Tick.Create(id, Enumerable.Empty<IData>());
            }

            var tick = Tick.Create(id, _datum);
            _datum = new List<IData>();

            return tick;
        }

        public void Reset()
        {
            _datum.Clear();
        }
    }
}
