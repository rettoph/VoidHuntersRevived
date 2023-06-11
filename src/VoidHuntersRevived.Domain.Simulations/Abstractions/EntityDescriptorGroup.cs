using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Simulations.Abstractions
{
    internal sealed class EntityDescriptorGroup
    {
        public readonly IEntityDescriptor Descriptor;
        public readonly ExclusiveGroup Group;

        public EntityDescriptorGroup(IEntityDescriptor descriptor, ExclusiveGroup group)
        {
            this.Descriptor = descriptor;
            this.Group = group;
        }
    }
}
