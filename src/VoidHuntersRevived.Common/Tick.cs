using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Library.Common
{
    public sealed class Tick : Message<Tick>
    {
        private readonly IEnumerable<ISimulationData> _data;
        private int? _count;

        public IEnumerable<ISimulationData> Data => _data;

        public int Count => _count ??= _data.Count();

        public int Id { get; }

        internal Tick(int id, IEnumerable<ISimulationData> data)
        {
            _data = data;

            this.Id = id;
        }

        public static Tick Empty(int id)
        {
            return new Tick(id, Enumerable.Empty<ISimulationData>());
        }

        public static Tick Create(int id, IEnumerable<ISimulationData> data)
        {
            return new Tick(id, data);
        }
    }
}
