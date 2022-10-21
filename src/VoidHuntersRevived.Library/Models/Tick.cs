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

namespace VoidHuntersRevived.Library.Models
{
    public class Tick : Message, IEnumerable<ITickData>, IMessage
    {
        private IEnumerable<ITickData> _datum;

        public const uint MinimumValidId = 1;

        public static readonly Tick Default = new Tick(0, Enumerable.Empty<ITickData>());

        public readonly uint Id;

        internal Tick(uint id, IEnumerable<ITickData> datum)
        {
            this.Id = id;

            _datum = datum;
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
