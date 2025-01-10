using VoidHuntersRevived.Domain.Entities.Common;

namespace LiteNetLib.Utils
{
    public static class NetDataWriterExtensions
    {
        public static void Put(this NetDataWriter writer, EntityGlobalId value) => writer.Put(value.Value);
    }
}