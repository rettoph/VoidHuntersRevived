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
        private IList<ISimulationInputData> _datum;

        public ServerTickFactory()
        {
            _datum = new List<ISimulationInputData>();
        }

        public void Enqueue(ISimulationInputData data)
        {
            _datum.Add(data);
        }

        public Tick Create(int id)
        {
            if (_datum.Count == 0)
            {
                return Tick.Create(id, Enumerable.Empty<ISimulationInputData>());
            }

            var tick = Tick.Create(id, _datum);
            _datum = new List<ISimulationInputData>();

            return tick;
        }

        public void Reset()
        {
            _datum.Clear();
        }
    }
}
