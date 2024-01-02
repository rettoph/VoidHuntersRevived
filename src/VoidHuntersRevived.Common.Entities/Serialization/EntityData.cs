using VoidHuntersRevived.Common.Core;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public class EntityData
    {
        public readonly VhId Id;
        public readonly byte[] Bytes;

        internal EntityData(VhId id, byte[] bytes)
        {
            Id = id;
            Bytes = bytes;
        }
    }
}
