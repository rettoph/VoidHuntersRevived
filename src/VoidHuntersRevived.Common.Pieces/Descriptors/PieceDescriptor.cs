using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Resources;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public abstract class PieceDescriptor : VoidHuntersEntityDescriptor
    {
        public PieceDescriptor()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<Node>(
                    builder: new ComponentBuilder<Node>(),
                    serializer: new ComponentSerializer<Node>(
                        writer: (writer, node) =>
                        {
                            writer.Write(node.TreeId);
                        },
                        reader: (seed, reader) =>
                        {
                            // If no seed is passed the tree should be read, if a seed is passed we assume it is the id of the owning tree
                            // This is relevant during Node deletion revision and Tree creation from cloned data within TreeFactory
                            VhId treeId = reader.ReadVhId(seed);
                            treeId = seed.Value == VhId.Empty.Value ? treeId : seed;
                            return  new Node(treeId);
                        }))
            });
        }
    }
}
