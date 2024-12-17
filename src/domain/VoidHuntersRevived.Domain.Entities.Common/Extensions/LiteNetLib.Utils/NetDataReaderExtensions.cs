using VoidHuntersRevived.Domain.Entities.Common;

namespace LiteNetLib.Utils
{
    public static class NetDataReaderExtensions
    {
        public static EntityGlobalId GetEntityGlobalId(this NetDataReader reader)
        {
            return new EntityGlobalId(reader.GetVhId());
        }
    }
}
