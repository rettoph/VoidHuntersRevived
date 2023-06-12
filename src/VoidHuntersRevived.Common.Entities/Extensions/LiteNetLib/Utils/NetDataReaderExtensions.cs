using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

namespace LiteNetLib.Utils
{
    public static class NetDataReaderExtensions
    {
        public static void GetEntityId(this NetDataReader reader, out EntityId id)
        {
            id = new EntityId(reader.GetUInt128());
        }

        public static EntityId GetEntityId(this NetDataReader reader)
        {
            return new EntityId(reader.GetUInt128());
        }
    }
}
