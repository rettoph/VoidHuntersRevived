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

namespace VoidHuntersRevived.Common.Physics.Descriptors
{
    public abstract class BodyDescriptor : VoidHuntersEntityDescriptor
    {
        public BodyDescriptor()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<Collision>(
                    builder: new ComponentBuilder<Collision>(),
                    serializer: DefaultComponentSerializer<Collision>.Default),
                new ComponentManager<Location>(
                    builder: new ComponentBuilder<Location>(),
                    serializer: DefaultComponentSerializer<Location>.Default)
            });
        }
    }
}
