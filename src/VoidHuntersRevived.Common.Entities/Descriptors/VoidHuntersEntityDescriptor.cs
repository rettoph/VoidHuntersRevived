using Svelto.ECS;
using Svelto.ECS.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Domain.Common.Components;

namespace VoidHuntersRevived.Common.Entities.Descriptors
{
    public abstract class VoidHuntersEntityDescriptor : IDynamicEntityDescriptor
    {
        private DynamicEntityDescriptor<BaseEntityDescriptor> _dynamicDescriptor;
        private readonly List<ComponentManager> _componentManagers;

        public IComponentBuilder[] componentsToBuild => _dynamicDescriptor.componentsToBuild;

        public IEnumerable<ComponentManager> ComponentManagers => _componentManagers;

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

        public void Serialize(EntityWriter writer, EGID egid, EntitiesDB entities)
        {
            entities.QueryEntitiesAndIndex<EntityVhId>(egid, out uint index);

            foreach (ComponentManager componentManager in _componentManagers)
            {
                componentManager.Serializer.Serialize(writer, index, egid.groupID, entities);
            }
        }

        public void Deserialize(EntityReader reader, ref EntityInitializer initializer)
        {
            foreach (ComponentManager componentManager in _componentManagers)
            {
                componentManager.Serializer.Deserialize(reader, ref initializer);
            }
        }
    }
}
