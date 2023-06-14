using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities.Abstractions
{
    internal sealed class EntityDescriptor : IEntityDescriptor
    {
        private readonly IComponentBuilder[] _componentsToBuild = new[]
        {
            new ComponentBuilder<EntityVhId>()
        };

        public readonly ExclusiveGroup ExclusiveGroup = new ExclusiveGroup();

        public IComponentBuilder[] componentsToBuild => _componentsToBuild;

        public EntityDescriptor()
        {

        }
    }
}
