using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Common.Components;

namespace VoidHuntersRevived.Common.Entities.Descriptors
{
    public class BaseEntityDescriptor : IEntityDescriptor
    {
        private readonly IComponentBuilder[] _componentsToBuild = new[]
        {
            new ComponentBuilder<EntityVhId>()
        };

        public IComponentBuilder[] componentsToBuild => _componentsToBuild;
    }
}
