using Guppy.Common;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Messages
{
    public class Tick : Message, IEnumerable<ITickData>, IMessage
    {
        private IEnumerable<ITickData> _datum;

        public const int MinimumValidId = 1;

        public static readonly Tick Default = Empty(0);

        public readonly int Id;

        internal Tick(int id, IEnumerable<ITickData> datum)
        {
            this.Id = id;

            _datum = datum;
        }

        public static Tick Empty(int id)
        {
            return new Tick(id, Enumerable.Empty<ITickData>());
        }

        public IEnumerator<ITickData> GetEnumerator()
        {
            return _datum.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
