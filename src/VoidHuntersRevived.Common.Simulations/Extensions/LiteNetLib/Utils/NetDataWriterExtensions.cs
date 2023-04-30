using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace LiteNetLib.Utils
{
    public static class NetDataWriterExtensions
    {
        public static unsafe void Put(this NetDataWriter writer, in ParallelKey key)
        {
            var test = Guid.NewGuid();
            ulong* longs = (ulong*)&test;
            writer.Put(key.Hash);
        }

        public static unsafe void Put(this NetDataWriter writer, ParallelKey key)
        {
            writer.Put(key.Hash);
        }
    }
}
