using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common
{
    public struct ParallelKey
    {
        public readonly string Type;
        public readonly int Value;

        public ParallelKey(string type, int value)
        {
            this.Type = type;
            this.Value = value;
        }

        public static ParallelKey From(string type, int value)
        {
            return new ParallelKey(type, value);
        }

        public static ParallelKey From(string type, object value)
        {
            return new ParallelKey(type, value.GetHashCode());
        }
    }
}
