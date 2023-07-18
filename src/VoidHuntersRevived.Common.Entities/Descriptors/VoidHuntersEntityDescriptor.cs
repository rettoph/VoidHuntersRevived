using Svelto.DataStructures;
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
using VoidHuntersRevived.Common.Entities.Utilities;
using VoidHuntersRevived.Domain.Common.Components;

namespace VoidHuntersRevived.Common.Entities.Descriptors
{
    public abstract class VoidHuntersEntityDescriptor : IDynamicEntityDescriptor
    {
        private DynamicEntityDescriptor<BaseEntityDescriptor> _dynamicDescriptor;
        private readonly List<ComponentManager> _componentManagers;
        private readonly FasterList<ComponentDisposer> _disposers;

        public IComponentBuilder[] componentsToBuild => _dynamicDescriptor.componentsToBuild;

        public IEnumerable<ComponentManager> ComponentManagers => _componentManagers;

        protected VoidHuntersEntityDescriptor()
        {
            _dynamicDescriptor = DynamicEntityDescriptor<BaseEntityDescriptor>.CreateDynamicEntityDescriptor();
            _componentManagers = new List<ComponentManager>();
            _disposers = new FasterList<ComponentDisposer>();
        }

        protected VoidHuntersEntityDescriptor ExtendWith(ComponentManager[] managers)
        {
            var builders = managers.Select(x => x.Builder).ToArray();
            _dynamicDescriptor.ExtendWith(builders);

            foreach(ComponentManager manager in managers)
            {
                _componentManagers.Add(manager);
                
                if(manager.Disposer is not null)
                {
                    _disposers.Add(manager.Disposer);
                }
            }

            return this;
        }

        public void Serialize(EntityWriter writer, EGID egid, EntitiesDB entities, uint index)
        {
            foreach (ComponentManager componentManager in _componentManagers)
            {
                componentManager.Serializer.Serialize(writer, index, egid.groupID, entities);
            }
        }

        public void Deserialize(in VhId seed, EntityReader reader, ref EntityInitializer initializer)
        {
            foreach (ComponentManager componentManager in _componentManagers)
            {
                componentManager.Serializer.Deserialize(in seed, reader, ref initializer);
            }
        }

        public void Dispose(in EGID egid, EntitiesDB entities)
        {
            if(_disposers.count == 0)
            {
                return;
            }


            entities.QueryEntitiesAndIndex<EntityVhId>(egid, out uint index);

            foreach (ComponentDisposer disposer in _disposers)
            {
                disposer.Dispose(index, egid.groupID, entities);
            }
        }
    }
}
