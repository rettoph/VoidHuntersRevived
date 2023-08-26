using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VoidHuntersRevived.Common.Entities;
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

        protected override Node Read(EntityReader reader, EntityId id)
        {
            // If no seed is passed the tree should be read, if a seed is passed we assume it is the id of the owning tree
            // This is relevant during Node deletion revision and Tree creation from cloned data within TreeFactory
            VhId treeVhId = reader.ReadVhId();
            treeVhId = reader.Seed.Value == VhId.Empty.Value ? treeVhId : reader.Seed;

            return new Node(id, _entities.GetId(treeVhId));
        }

        protected override void Write(EntityWriter writer, Node instance)
        {
            writer.Write(instance.TreeId.VhId);
        }
    }
}
