using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Components
{
    public sealed class Parallelable
    {
        private Dictionary<SimulationType, int> _ids;

        public readonly ParallelKey Key;

        public int this[SimulationType simulation] => _ids[simulation];

        public Parallelable(ParallelKey key)
        {
            this.Key = key;

            _ids = new Dictionary<SimulationType, int>();
        }

        public int GetId(SimulationType simulation)
        {
            return _ids[simulation];
        }

        public bool TryGetId(SimulationType simulation, out int id)
        {
            return _ids.TryGetValue(simulation, out id);
        }

        public void AddId(ISimulation simulation, int id)
        {
            _ids.Add(simulation.Type, id);
        }

        public void RemoveId(ISimulation simulation)
        {
            _ids.Remove(simulation.Type);
        }
    }
}
