using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    internal sealed class TreeSerializer : ComponentSerializer<Tree>
    {
        private readonly IEntityService _entities;

        public TreeSerializer(IEntityService entities)
        {
            _entities = entities;
        }

        protected override Tree Read(EntityReader reader, EntityId id)
        {
            return new Tree(_entities.Deserialize(reader, null));
        }

        protected override void Write(EntityWriter writer, Tree instance)
        {
            _entities.Serialize(instance.HeadId, writer);
        }
    }
}
