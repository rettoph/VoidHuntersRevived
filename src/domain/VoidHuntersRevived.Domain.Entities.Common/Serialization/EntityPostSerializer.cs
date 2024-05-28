using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;

namespace VoidHuntersRevived.Domain.Entities.Common.Serialization
{
    [Service<EntityPostSerializer>(ServiceLifetime.Scoped, true)]
    public abstract class EntityPostSerializer
    {
        public readonly Type Type;

        internal EntityPostSerializer(Type type)
        {
            Type = type;
        }

        public abstract void Serialize(EntityWriter writer, in GroupIndex groupIndex, EntitiesDB entitiesDB, in SerializationOptions options);
        public abstract void Deserialize(in VhId sourceId, in DeserializationOptions options, EntityReader reader, in EntityId id);
    }
}
