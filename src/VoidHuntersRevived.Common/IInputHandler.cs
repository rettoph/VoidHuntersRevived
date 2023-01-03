using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common
{
    public interface IInputHandler
    {
    }

    public interface IInputHandler<TData> : IInputHandler
        where TData : ISimulationEvent
    {
        void Invoke(NetPeer? sender, TData data, ISimulation simulation);
    }
}
