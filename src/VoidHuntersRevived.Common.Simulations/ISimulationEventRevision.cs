using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulationEventRevision : IMessage
    {
        ParallelKey Key { get; }

        int SenderId { get; }

        ISimulation Simulation { get; }

        object Body { get; }

        object? Response { get; }
    }

    public interface ISimulationEventRevision<TSimulationEventData> : ISimulationEventRevision
    {
        new TSimulationEventData Body { get; }
    }
}
