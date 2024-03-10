using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Serialization
{
    public class EntityData
    {
        public static EntityData Default = new EntityData(default, Array.Empty<long>(), Array.Empty<byte>());

        public readonly VhId Id;
        public readonly long[] Positions;
        public readonly byte[] Bytes;

        internal EntityData(VhId id, long[] positions, byte[] bytes)
        {
            Id = id;
            Positions = positions;
            Bytes = bytes;
        }
    }
}
