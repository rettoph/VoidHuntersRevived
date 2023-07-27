using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Physics.Descriptors;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public abstract class TreeDescriptor : BodyDescriptor
    {
        public TreeDescriptor()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<Tree>(
                    builder: new ComponentBuilder<Tree>(),
                    serializer: new ComponentSerializer<Tree>(
                        writer: (entities, writer, tree) =>
                        {
                            entities.Serialize(tree.HeadId, writer);
                        },
                        reader: (entities, reader, id) =>
                        {
                            return new Tree(entities.Deserialize(reader, null));
                        }))
            });
        }
    }
}
