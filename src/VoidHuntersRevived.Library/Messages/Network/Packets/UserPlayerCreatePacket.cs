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
    public sealed class UserPlayerCreatePacket : IData
    {
        public Int32 UserId { get; init; }

        internal static UserPlayerCreatePacket Read(NetDataReader reader, NetworkProvider network)
        {
            return new UserPlayerCreatePacket()
            {
                UserId = reader.GetInt()
            };
        }

        internal static void Write(NetDataWriter writer, NetworkProvider network, UserPlayerCreatePacket packet)
        {
            writer.Put(packet.UserId);
        }
    }
}
