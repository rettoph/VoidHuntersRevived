using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityDescriptorService : IEntityDescriptorService
    {
        private Dictionary<Guid, VoidHuntersEntityDescriptor> _descriptors;
        private Dictionary<Guid, VoidHuntersEntityDescriptorSpawner> _spawners;
        private Dictionary<Guid, VoidHuntersEntityDescriptorSpawner> _instanceSpawners;

        public EntityDescriptorService(IEnumerable<VoidHuntersEntityDescriptor> descriptors)
        {
            _descriptors = descriptors.ToDictionary(x => x.Id.Value, x => x);
            _spawners = _descriptors.Values.ToDictionary(x => x.Id.Value, VoidHuntersEntityDescriptorSpawner.Build);
            _instanceSpawners = new Dictionary<Guid, VoidHuntersEntityDescriptorSpawner>();
        }

        public VoidHuntersEntityDescriptor GetById(VhId id)
        {
            return _descriptors[id.Value];
        }

        public VoidHuntersEntityDescriptor GetByEntityVhId(VhId id)
        {
            return _instanceSpawners[id.Value].Descriptor;
        }

        public EntityInitializer Spawn(VoidHuntersEntityDescriptor descriptor, IEntityFactory factory, VhId vhid, out EntityId id)
        {
            VoidHuntersEntityDescriptorSpawner spawner = _spawners[descriptor.Id.Value];
            EntityInitializer initializer = spawner.Spawn(factory, vhid, out id);

            _instanceSpawners.Add(vhid.Value, spawner);

            return initializer;
        }

        public void Despawn(IEntityFunctions functions, in EntityId id)
        {
            _instanceSpawners.Remove(id.VhId.Value, out var spawner);
            spawner!.Despawn(functions, id.EGID);
        }
    }
}
