using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Factories
{
    public interface ITickFactory
    {
        void Enqueue(ISimulationEvent data);

        Tick Create(int id);
    }
}
