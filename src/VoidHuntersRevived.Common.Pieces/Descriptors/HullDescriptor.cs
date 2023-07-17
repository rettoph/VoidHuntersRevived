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
    public class HullDescriptor : PieceDescriptor
    {
        public HullDescriptor()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<ResourceId<Visible>>(
                    builder: new ComponentBuilder<ResourceId<Visible>>(),
                    serializer: ComponentSerializer<ResourceId<Visible>>.Default),
                new ComponentManager<ResourceId<Rigid>>(
                    builder: new ComponentBuilder<ResourceId<Rigid>>(),
                    serializer: ComponentSerializer<ResourceId<Rigid>>.Default),
                new ComponentManager<Joints>(
                    builder: new ComponentBuilder<Joints>(),
                    serializer: new ComponentSerializer<Joints>(
                        writer: (writer, joints) =>
                        {
                            writer.Write(joints.Items.count);
                        },
                        reader: (seed, reader) =>
                        {
                            var joints =  new Joints(reader.ReadUInt32());

                            return joints;
                        }))
            });
        }
    }
}
