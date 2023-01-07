using Guppy.Common;
using Guppy.Network.Enums;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulationEvent : IMessage
    {
        SimulationType Source { get; }
        ISimulation Simulation { get; }
        object Data { get; }
    }

    public interface ISimulationEvent<TData> : ISimulationEvent
        where TData : notnull, ISimulationData
    {
        new TData Data { get; }
    }
}
