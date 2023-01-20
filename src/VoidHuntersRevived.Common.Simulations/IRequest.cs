using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface IRequest<TData> : IMessage
        where TData : IData
    {
        ParallelKey PilotKey { get; }

        TData Data { get; }

        void Reject();
    }
}
