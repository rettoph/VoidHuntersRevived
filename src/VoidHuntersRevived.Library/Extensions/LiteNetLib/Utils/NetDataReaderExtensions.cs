using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Structs;

namespace LiteNetLib.Utils
{
    public static class NetDataReaderExtensions
    {
        public static ConnectionNodeNetworkId? GetConnectionNodeNetworkdId(this NetDataReader reader)
        {
            if (reader.GetIf())
            {
                return new ConnectionNodeNetworkId(
                    reader.GetUShort(),
                    reader.GetByte());
            }

            return default;
        }
    }
}
