using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface IInput : IMessage
    {
        Guid Id { get; }
        ISimulation Simulation { get; }
        ParallelKey Sender { get; }
        IData Data { get; }
    }

    public interface IInput<TData> : IInput
        where TData : notnull, IData
    {
        new TData Data { get; }
    }
}
