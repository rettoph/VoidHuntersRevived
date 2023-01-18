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
        private readonly IEnumerable<UserInput> _inputs;
        private int? _count;

        public IEnumerable<UserInput> Inputs => _inputs;

        public int Count => _count ??= _inputs.Count();

        public int Id { get; }

        internal Tick(int id, IEnumerable<UserInput> data)
        {
            _inputs = data;

            this.Id = id;
        }

        public static Tick Empty(int id)
        {
            return new Tick(id, Enumerable.Empty<UserInput>());
        }

        public static Tick Create(int id, IEnumerable<UserInput> data)
        {
            return new Tick(id, data);
        }
    }
}
