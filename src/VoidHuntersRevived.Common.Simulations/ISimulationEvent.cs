﻿using Standart.Hash.xxHash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Enums;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulationEvent
    {
        ParallelKey Key { get; }

        int SenderId { get; }

        ISimulation Simulation { get; }

        object Body { get; }

        SimulationEventResult Result { get; }

        ParallelKey NewKey();

        ParallelKey PreviousKey();
    }

    public interface ISimulationEvent<TSimulationEventData> : ISimulationEvent
    {
        new TSimulationEventData Body { get; }
    }
}
