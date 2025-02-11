using VoidHuntersRevived.Domain.Entities.Common;

namespace LiteNetLib.Utils
{
    public static class NetDataReaderExtensions
    {
        public static EntityGlobalId GetEntityGlobalId(this NetDataReader reader)
        {
            return new(reader.GetVhId());
        }

        public static Id<T> GetId<T>(this NetDataReader reader)
        {
            return new Id<T>(reader.GetVhId());
        }
    }
}