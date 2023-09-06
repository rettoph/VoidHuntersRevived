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
using VoidHuntersRevived.Common.Physics.Serialization.Components;

namespace VoidHuntersRevived.Common.Physics.Descriptors
{
    public abstract class BodyDescriptor : VoidHuntersEntityDescriptor
    {
        public BodyDescriptor() : base()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<Collision, CollisionComponentSerializer>(),
                new ComponentManager<Location, LocationComponentSerializer>(),
                new ComponentManager<Awake, AwakeComponentSerializer>(),
            });
        }
    }
}
