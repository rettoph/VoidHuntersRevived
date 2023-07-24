using Microsoft.Xna.Framework;
using Svelto.Common;
using Svelto.DataStructures;
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
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public class HullDescriptor : PieceDescriptor
    {
        public HullDescriptor()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<Joints>(
                    builder: new ComponentBuilder<Joints>(),
                    serializer: new ComponentSerializer<Joints>(
                        writer: (entities, writer, instance) =>
                        {
                            writer.WriteStruct(instance.Child);
                            writer.WriteNativeDynamicArray(instance.Parents);
                        },
                        reader: (entities, reader, id) => new Joints()
                        {
                            Child = reader.ReadStruct<Joint>(),
                            Parents = reader.ReadNativeDynamicArray<Joint>(),
                        }))
            });
        }
    }
}
