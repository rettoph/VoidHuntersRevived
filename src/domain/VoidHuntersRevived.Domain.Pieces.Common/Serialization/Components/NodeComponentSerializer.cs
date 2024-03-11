using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Common.Serialization.Components
{
    [AutoLoad]
    public sealed class NodeComponentSerializer : ComponentSerializer<Node>
    {
        private readonly IEntityService _entities;

        public NodeComponentSerializer(IEntityService entities)
        {
            _entities = entities;
        }

        protected override Node Read(in DeserializationOptions options, EntityReader reader, in EntityId id)
        {
            return new Node(id, _entities.GetId(options.Owner));
        }

        protected override void Write(EntityWriter writer, in Node instance, in SerializationOptions options)
        {
        }
    }
}
