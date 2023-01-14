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
        private readonly IEnumerable<IData> _data;
        private int? _count;

        public IEnumerable<IData> Data => _data;

        public int Count => _count ??= _data.Count();

        public int Id { get; }

        internal Tick(int id, IEnumerable<IData> data)
        {
            _data = data;

            this.Id = id;
        }

        public static Tick Empty(int id)
        {
            return new Tick(id, Enumerable.Empty<IData>());
        }

        public static Tick Create(int id, IEnumerable<IData> data)
        {
            return new Tick(id, data);
        }
    }
}
