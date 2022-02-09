using Guppy.Network;
using Guppy.Network.Interfaces;
using Guppy.Threading.Interfaces;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Messages.Network.Packets
{
    internal sealed class ShipCreatePacket : IData
    {
        public UInt16 ChainNetworkId { get; init; }
        public UInt16? PlayerNetworkId { get; init; }

        internal static ShipCreatePacket Read(NetDataReader reader, NetworkProvider network)
        {
            return new ShipCreatePacket()
            {
                ChainNetworkId = reader.GetUShort(),
                PlayerNetworkId = reader.GetBool() ? reader.GetUShort() : null
            };
        }

        internal static void Write(NetDataWriter reader, NetworkProvider network, ShipCreatePacket packet)
        {
            reader.Put(packet.ChainNetworkId);

            if(reader.PutIf(packet.PlayerNetworkId.HasValue))
            {
                reader.Put(packet.PlayerNetworkId.Value);
            }
        }
    }
}
