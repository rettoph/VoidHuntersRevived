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
    public sealed class WorldObjectPositionPacket : IData
    {
        public Vector2 Position { get; init; }
        public Single Rotation { get; init; }

        internal static WorldObjectPositionPacket Read(NetDataReader reader, NetworkProvider network)
        {
            return new WorldObjectPositionPacket()
            {
                Position = reader.GetVector2(),
                Rotation = reader.GetFloat()
            };
        }

        internal static void Write(NetDataWriter writer, NetworkProvider network, WorldObjectPositionPacket packet)
        {
            writer.Put(packet.Position);
            writer.Put(packet.Rotation);
        }
    }
}
