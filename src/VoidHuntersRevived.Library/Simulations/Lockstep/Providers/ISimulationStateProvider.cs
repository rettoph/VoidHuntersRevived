using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Providers
{
    public interface ISimulationStateProvider
    {
        public class LockstepData
        {
            public readonly ISimulation Simulation;
            public readonly State State;
            public readonly NetScope Scope;

            public LockstepData(ISimulation simulation, State state, NetScope scope)
            {
                this.Simulation = simulation;
                this.State = state;
                this.Scope = scope;
            }
        }

        public IEnumerable<LockstepData> Items { get; }

        void Add(ISimulation simulation, IServiceProvider provider);
        void Remove(ISimulation simulation);
    }
}
