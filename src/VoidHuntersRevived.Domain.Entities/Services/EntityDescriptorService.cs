using Autofac;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityDescriptorService : IScopedEntityDescriptorService
    {
        private Dictionary<Guid, ScopedVoidHuntersEntityDescriptor> _descriptors;
        private Dictionary<Guid, ScopedVoidHuntersEntityDescriptor> _instanceDescriptors;

        public EntityDescriptorService(ILifetimeScope scope, IEnumerable<VoidHuntersEntityDescriptor> descriptors)
        {
            _descriptors = descriptors.ToDictionary(x => x.Id.Value, x => ScopedVoidHuntersEntityDescriptor.Build(x, scope));
            _instanceDescriptors = new Dictionary<Guid, ScopedVoidHuntersEntityDescriptor>();
        }

        public ScopedVoidHuntersEntityDescriptor GetById(VhId id)
        {
            return _descriptors[id.Value];
        }

        public ScopedVoidHuntersEntityDescriptor GetByEntityVhId(VhId id)
        {
            return _instanceDescriptors[id.Value];
        }

        public EntityInitializer Spawn(VoidHuntersEntityDescriptor descriptor, IEntityFactory factory, VhId vhid, out EntityId id)
        {
            ScopedVoidHuntersEntityDescriptor spawner = _descriptors[descriptor.Id.Value];
            EntityInitializer initializer = spawner.Spawn(factory, vhid, out id);

            _instanceDescriptors.Add(vhid.Value, spawner);

            return initializer;
        }

        public void Despawn(IEntityFunctions functions, in EntityId id)
        {
            _instanceDescriptors.Remove(id.VhId.Value, out var spawner);
            spawner!.Despawn(functions, id.EGID);
        }
    }
}
