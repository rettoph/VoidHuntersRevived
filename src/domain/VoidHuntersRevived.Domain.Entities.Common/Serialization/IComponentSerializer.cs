using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;

namespace VoidHuntersRevived.Domain.Entities.Common.Serialization
{
    public interface IComponentSerializer
    {
        Type Type { get; }

        void Serialize(ref EntityWriter writer, in EntityId id, in GroupIndex groupIndex, EntitiesDB entitiesDB, in SerializationOptions options);
        void Deserialize(in VhId sourceId, in DeserializationOptions options, ref EntityReader reader, ref EntityInitializer initializer, in EntityId id);
    }
}
