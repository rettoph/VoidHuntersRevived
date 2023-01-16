using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common
{
    public readonly struct ParallelKey
    {
        public readonly string Type;
        public readonly int Value;
        public readonly int Index;

        private ParallelKey(string type, int value, int index)
        {
            this.Type = type;
            this.Value = value;
            this.Index = index;
        }

        public static ParallelKey From(string type, int value)
        {
            return new ParallelKey(type, value, 0);
        }

        public static ParallelKey From(string type, object value)
        {
            return new ParallelKey(type, value.GetHashCode(), 0);
        }

        public static ParallelKey From(string type, int value, int index)
        {
            return new ParallelKey(type, value, index);
        }

        public static ParallelKey From(string type, object value, int index)
        {
            return new ParallelKey(type, value.GetHashCode(), index);
        }
    }
}
