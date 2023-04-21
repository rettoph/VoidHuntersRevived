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
    public interface ITickFactory
    {
        void Enqueue(EventDto @event);

        Tick Create(int id);

        void Reset();
    }
}
