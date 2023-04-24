﻿using Guppy.Common;
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
        private readonly IEnumerable<InputDto> _inputs;
        private int? _count;

        public IEnumerable<InputDto> Inputs => _inputs;

        public int Count => _count ??= _inputs.Count();

        public int Id { get; }

        internal Tick(int id, IEnumerable<InputDto> inputs)
        {
            _inputs = inputs;

            this.Id = id;
        }

        public static Tick Empty(int id)
        {
            return new Tick(id, Enumerable.Empty<InputDto>());
        }

        public static Tick Create(int id, IEnumerable<InputDto> events)
        {
            return new Tick(id, events);
        }
    }
}
