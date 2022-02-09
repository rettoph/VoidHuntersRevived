using Guppy.Network;
using Guppy.Threading.Interfaces;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Messages.Network.Packets
{
    public class AetherBodyWorldObjectVelocityPacket : IData
    {
        public Vector2 LinearVelocity { get; init; }
        public Single AngularVelocity { get; init; }

        internal static AetherBodyWorldObjectVelocityPacket Read(NetDataReader reader, NetworkProvider network)
        {
            return new AetherBodyWorldObjectVelocityPacket()
            {
                LinearVelocity = reader.GetVector2(),
                AngularVelocity = reader.GetFloat()
            };
        }

        internal static void Write(NetDataWriter writer, NetworkProvider network, AetherBodyWorldObjectVelocityPacket packet)
        {
            writer.Put(packet.LinearVelocity);
            writer.Put(packet.AngularVelocity);
        }
    }
}
