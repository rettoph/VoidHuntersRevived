using VoidHuntersRevived.Domain.Entities.Common;

namespace LiteNetLib.Utils
{
    public static class NetDataReaderExtensions
    {
        public static EntityGlobalId GetEntityGlobalId(this NetDataReader reader) => new(reader.GetVhId());
    }
}