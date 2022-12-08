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

        private readonly IEnumerable<ITickData> _data;
        private int? _count;

        public int Id { get; }

        public IEnumerable<ITickData> Data => _data;

        public int Count => _count ??= _data.Count();

        internal Tick(int id, IEnumerable<ITickData> datum)
        {
            this.Id = id;

            _data = datum;
        }

        public static Tick Empty(int id)
        {
            return new Tick(id, Enumerable.Empty<ITickData>());
        }
    }
}
