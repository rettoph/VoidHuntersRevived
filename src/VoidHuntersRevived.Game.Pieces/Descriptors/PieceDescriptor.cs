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
using VoidHuntersRevived.Game.Pieces.Components;
using VoidHuntersRevived.Game.Pieces.Resources;

namespace VoidHuntersRevived.Game.Pieces.Descriptors
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
                        writer: (writer, tree) =>
                        {
                            writer.Write(tree.TreeId);
                        },
                        reader: reader =>
                        {
                            return new Node(reader.ReadVhId());
                        }))
            });
        }
    }
}
