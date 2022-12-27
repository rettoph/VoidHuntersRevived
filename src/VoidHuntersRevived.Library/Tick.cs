using Guppy.Common;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library
{
    public class Tick : Message, IMessage
    {
        public const int MinimumValidId = 1;
        public const int MaximumInvalidId = MinimumValidId - 1;

        private readonly IEnumerable<ISimulationEvent> _events;
        private int? _count;

        public int Id { get; }

        public IEnumerable<ISimulationEvent> Events => _events;

        public int Count => _count ??= _events.Count();

        internal Tick(int id, IEnumerable<ISimulationEvent> datum)
        {
            this.Id = id;

            _events = datum;
        }

        public static Tick Empty(int id)
        {
            return new Tick(id, Enumerable.Empty<ISimulationEvent>());
        }
    }
}
