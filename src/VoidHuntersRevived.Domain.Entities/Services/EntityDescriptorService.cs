using Autofac;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Services;

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
            ScopedVoidHuntersEntityDescriptor scopedDescriptor = _descriptors[descriptor.Id.Value];
            EntityInitializer initializer = scopedDescriptor.Spawn(factory, vhid, out id);

            _instanceDescriptors.Add(vhid.Value, scopedDescriptor);

            return initializer;
        }

        public void SoftDespawn(IEntityService entities, in EntityId id)
        {
            _instanceDescriptors[id.VhId.Value].SoftDespawn(entities, in id);
        }

        public void HardDespawn(IEntityService entities, IEntityFunctions functions, in EntityId id)
        {
            _instanceDescriptors.Remove(id.VhId.Value, out var scopedDescriptor);
            scopedDescriptor!.HardDespawn(entities, functions, id);
        }
    }
}
