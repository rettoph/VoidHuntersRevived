using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Engines
{
    public interface IRevertEventEngine : IEventEngine
    {
    }

    public interface IRevertEventEngine<T> : IRevertEventEngine
        where T : IEventData
    {
        void Revert(VhId eventId, T data);
    }
}
