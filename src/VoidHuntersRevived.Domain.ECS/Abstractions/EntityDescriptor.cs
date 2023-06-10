using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS;

namespace VoidHuntersRevived.Domain.ECS.Abstractions
{
    internal sealed class EntityDescriptor : IEntityDescriptor
    {
        public readonly ExclusiveGroup ExclusiveGroup = new ExclusiveGroup();

        public IComponentBuilder[] componentsToBuild => Array.Empty<IComponentBuilder>();

        public EntityDescriptor()
        {

        }
    }
}
