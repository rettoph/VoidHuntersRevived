﻿using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Factories
{
    [PeerTypeFilter(PeerType.Server | PeerType.None)]
    internal sealed class DefaultTickFactory : ITickFactory
    {
        private IList<SimulationEventData> _inputs;

        public DefaultTickFactory()
        {
            _inputs = new List<SimulationEventData>();
        }

        public void Enqueue(SimulationEventData input)
        {
            _inputs.Add(input);
        }

        public Tick Create(int id)
        {
            if (_inputs.Count == 0)
            {
                return Tick.Create(id, Enumerable.Empty<SimulationEventData>());
            }

            var tick = Tick.Create(id, _inputs);
            _inputs = new List<SimulationEventData>();

            return tick;
        }

        public void Reset()
        {
            _inputs.Clear();
        }
    }
}
