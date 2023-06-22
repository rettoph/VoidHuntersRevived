using Svelto.ECS;
using Svelto.ECS.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Common.Components;

namespace VoidHuntersRevived.Common.Entities.Descriptors
{
    public abstract class VoidHuntersEntityDescriptor : IDynamicEntityDescriptor
    {
        private DynamicEntityDescriptor<BaseEntityDescriptor> _dynamicDescriptor;
        private readonly List<ComponentManager> _componentManagers;

        public IComponentBuilder[] componentsToBuild => _dynamicDescriptor.componentsToBuild;

        protected VoidHuntersEntityDescriptor()
        {
            _dynamicDescriptor = DynamicEntityDescriptor<BaseEntityDescriptor>.CreateDynamicEntityDescriptor();
            _componentManagers = new List<ComponentManager>();
        }

        protected VoidHuntersEntityDescriptor ExtendWith(ComponentManager[] components)
        {
            var builders = components.Select(x => x.Builder).ToArray();
            _dynamicDescriptor.ExtendWith(builders);
            _componentManagers.AddRange(components);

            return this;
        }

        public void Clone(EGID source, EntitiesDB entities, ref EntityInitializer clone)
        {
            entities.QueryEntitiesAndIndex<EntityVhId>(source, out uint index);

            foreach(ComponentManager componentManager in _componentManagers)
            {
                componentManager.Clone(index, source.groupID, entities, ref clone);
            }
        }
    }
}
