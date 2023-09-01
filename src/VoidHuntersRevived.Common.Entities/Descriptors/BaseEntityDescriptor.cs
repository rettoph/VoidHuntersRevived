using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Entities.Descriptors
{
    public class BaseEntityDescriptor : IEntityDescriptor
    {
        private readonly IComponentBuilder[] _componentsToBuild = new IComponentBuilder[]
        {
            new ComponentBuilder<EntityStatus>(),
            new ComponentBuilder<EntityId>(),
            new ComponentBuilder<DescriptorId>()
        };

        public IComponentBuilder[] componentsToBuild => _componentsToBuild;
    }
}
