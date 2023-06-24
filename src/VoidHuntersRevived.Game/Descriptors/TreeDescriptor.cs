using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Game.Pieces.Components;

namespace VoidHuntersRevived.Game.Descriptors
{
    public abstract class TreeDescriptor : VoidHuntersEntityDescriptor
    {
        public TreeDescriptor()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<Body>(
                    builder: new ComponentBuilder<Body>(),
                    serializer: ComponentSerializer<Body>.Default()),
                new ComponentManager<Tree>(
                    builder: new ComponentBuilder<Tree>(),
                    serializer: new ComponentSerializer<Tree>(
                        writer: (writer, tree) =>
                        {
                            writer.Write(tree.HeadId);
                        },
                        reader: reader =>
                        {
                            return new Tree()
                            {
                                HeadId = reader.ReadVhId()
                            };
                        }))
            });
        }
    }
}
