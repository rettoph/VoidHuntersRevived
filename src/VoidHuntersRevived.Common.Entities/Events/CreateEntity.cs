using VoidHuntersRevived.Common.ECS;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Events
{
    public class CreateEntity
    {
        public static readonly EventId Noise = EventId.From<CreateEntity>();

        public readonly EventId? Key;
        public readonly EntityType Type;

        public CreateEntity(EntityType type, EventId? key)
        {
            this.Type = type;
            this.Key = key;
        }
    }
}
