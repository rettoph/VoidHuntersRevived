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

        public event OnEventDelegate<Parallelable> OnEmpty;
        public event OnEventDelegate<Parallelable, int> OnAdded;
        public event OnEventDelegate<Parallelable, int> OnRemoved;

        public Parallelable(
            ParallelKey key, 
            OnEventDelegate<Parallelable, int> onAdded, 
            OnEventDelegate<Parallelable, int> onRemoved, 
            OnEventDelegate<Parallelable> onEmpty)
        {
            this.Key = key;

            _ids = new Dictionary<SimulationType, int>();

            this.OnAdded = onAdded;
            this.OnRemoved = onRemoved;
            this.OnEmpty = onEmpty;
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
            this.OnAdded(this, id);
        }

        public void RemoveId(ISimulation simulation)
        {
            _ids.Remove(simulation.Type, out int id);
            this.OnRemoved(this, id);

            if (_ids.Count == 0)
            {
                this.OnEmpty.Invoke(this);
            }
        }
    }
}
