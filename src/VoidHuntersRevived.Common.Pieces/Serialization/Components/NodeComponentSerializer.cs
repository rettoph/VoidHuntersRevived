using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
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
