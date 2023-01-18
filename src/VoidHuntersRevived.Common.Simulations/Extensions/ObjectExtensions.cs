using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Extensions
{
    public static class ObjectExtensions
    {
        public static ParallelKey GetKey(this object value, string type)
        {
            return ParallelKey.From(type, value);
        }

        public static ParallelKey GetKey(this object value, string type, int index)
        {
            return ParallelKey.From(type, value, index);
        }
    }
}
