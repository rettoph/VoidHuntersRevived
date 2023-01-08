using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Common.Simulations.Lockstep
{
    public sealed class Tick : Message<Tick>
    {
        private readonly IEnumerable<ISimulationInputData> _data;
        private int? _count;

        public IEnumerable<ISimulationInputData> Data => _data;

        public int Count => _count ??= _data.Count();

        public int Id { get; }

        internal Tick(int id, IEnumerable<ISimulationInputData> data)
        {
            _data = data;

            this.Id = id;
        }

        public static Tick Empty(int id)
        {
            return new Tick(id, Enumerable.Empty<ISimulationInputData>());
        }

        public static Tick Create(int id, IEnumerable<ISimulationInputData> data)
        {
            return new Tick(id, data);
        }
    }
}
