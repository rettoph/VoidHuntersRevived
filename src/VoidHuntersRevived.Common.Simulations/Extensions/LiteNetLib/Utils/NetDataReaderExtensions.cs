using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace LiteNetLib.Utils
{
    public static class NetDataReaderExtensions
    {
        public static void GetEventId(this NetDataReader reader, out EventId id)
        {
            id = new EventId(reader.GetUInt128());
        }

        public static EventId GetEventId(this NetDataReader reader)
        {
            return new EventId(reader.GetUInt128());
        }
    }
}
