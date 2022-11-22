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

namespace VoidHuntersRevived.Library.Messages
{
    public class Tick : Message, IMessage
    {
        private readonly IEnumerable<ITickData> _data;

        public const int MinimumValidId = 1;

        public static readonly Tick Default = Empty(0);

        public int Id { get; }

        public IEnumerable<ITickData> Data => _data;

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
