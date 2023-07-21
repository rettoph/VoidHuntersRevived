using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ECS.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Domain.Common.Components;

namespace VoidHuntersRevived.Common.Entities.Descriptors
{
    public abstract class VoidHuntersEntityDescriptor : IDynamicEntityDescriptor
    {
        private DynamicEntityDescriptor<BaseEntityDescriptor> _dynamicDescriptor;
        private readonly List<ComponentManager> _componentManagers;
        private readonly VoidHuntersEntityDescriptorSpawner _spawner;

        public IComponentBuilder[] componentsToBuild => _dynamicDescriptor.componentsToBuild;

        public IEnumerable<ComponentManager> ComponentManagers => _componentManagers;

        protected VoidHuntersEntityDescriptor()
        {
            _dynamicDescriptor = DynamicEntityDescriptor<BaseEntityDescriptor>.CreateDynamicEntityDescriptor();
            _componentManagers = new List<ComponentManager>();
            _spawner = VoidHuntersEntityDescriptorSpawner.Build(this);
        }

        protected VoidHuntersEntityDescriptor ExtendWith(ComponentManager[] managers)
        {
            var builders = managers.Select(x => x.Builder).ToArray();
            _dynamicDescriptor.ExtendWith(builders);

            foreach(ComponentManager manager in managers)
            {
                _componentManagers.Add(manager);
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

        public void Deserialize(EntityReader reader, ref EntityInitializer initializer)
        {
            foreach (ComponentManager componentManager in _componentManagers)
            {
                componentManager.Serializer.Deserialize(reader, ref initializer);
            }
        }

        public EntityInitializer SpawnEntity(IEntityFactory factory, VhId vhid)
        {
            return _spawner.SpawnEntity(factory, vhid);
        }

        public void DespawnEntity(IEntityFunctions functions, in EGID egid)
        {
            _spawner.DespawnEntity(functions, in egid);
        }

        private abstract class VoidHuntersEntityDescriptorSpawner
        {
            public abstract EntityInitializer SpawnEntity(IEntityFactory factory, VhId vhid);
            public abstract void DespawnEntity(IEntityFunctions functions, in EGID egid);

            public static VoidHuntersEntityDescriptorSpawner Build(VoidHuntersEntityDescriptor descriptor)
            {
                Type spawnerType = typeof(VoidHuntersEntityDescriptorSpawner<>).MakeGenericType(descriptor.GetType());

                return (VoidHuntersEntityDescriptorSpawner)Activator.CreateInstance(spawnerType, descriptor)!;
            }
        }

        private sealed class VoidHuntersEntityDescriptorSpawner<TDescriptor> : VoidHuntersEntityDescriptorSpawner
            where TDescriptor : VoidHuntersEntityDescriptor, new()
        {
            private uint EntityId;
            public readonly ExclusiveGroup Group = new ExclusiveGroup();
            public readonly TDescriptor Descriptor;

            public VoidHuntersEntityDescriptorSpawner(TDescriptor descriptor)
            {
                Descriptor = descriptor;
            }

            public override EntityInitializer SpawnEntity(IEntityFactory factory, VhId vhid)
            {
                EGID egid = new EGID(EntityId++, Group);
                EntityInitializer initializer = factory.BuildEntity(egid, this.Descriptor);
                initializer.Init(new EntityVhId() { Value = vhid });

                return initializer;
            }

            public override void DespawnEntity(IEntityFunctions functions, in EGID egid)
            {
                functions.RemoveEntity<TDescriptor>(egid);
            }
        }
    }
}
