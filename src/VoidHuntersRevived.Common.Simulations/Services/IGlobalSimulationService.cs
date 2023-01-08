using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Services
{
    public interface IGlobalSimulationService
    {
        ReadOnlyCollection<ISimulation> Instances { get; }
        void Add(ISimulation instance);
        void Remove(ISimulation instance);
    }
}
