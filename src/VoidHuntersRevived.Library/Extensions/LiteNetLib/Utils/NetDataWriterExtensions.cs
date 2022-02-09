using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Structs;

namespace LiteNetLib.Utils
{
    public static class NetDataWriterExtensions
    {
        public static void Put(this NetDataWriter writer, ConnectionNodeNetworkId? networkId)
        {
            if (writer.PutIf(networkId.HasValue))
            {
                writer.Put(networkId.Value.OwnerNetworkId);
                writer.Put(networkId.Value.Index);
            }
        }
    }
}
