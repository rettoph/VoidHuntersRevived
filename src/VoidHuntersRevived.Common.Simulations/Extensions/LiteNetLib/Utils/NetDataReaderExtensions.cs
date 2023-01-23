using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace LiteNetLib.Utils
{
    public static class NetDataReaderExtensions
    {
        public static void GetParallelKey(this NetDataReader reader, out ParallelKey key)
        {
            key = new ParallelKey(reader.GetInt());
        }

        public static ParallelKey GetParallelKey(this NetDataReader reader)
        {
            return new ParallelKey(reader.GetInt());
        }
    }
}
