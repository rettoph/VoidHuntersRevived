using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityDescriptorService : IEntityDescriptorService
    {
        private readonly Dictionary<Id<VoidHuntersEntityDescriptor>, VoidHuntersEntityDescriptor> _ids;
        private readonly Dictionary<ExclusiveGroupStruct, VoidHuntersEntityDescriptor> _groups;

        public EntityDescriptorService(IEntityTypeService types)
        {
            _ids = types.GetAll().Select(x => x.Descriptor).Distinct().ToDictionary(x => x.Id, x => x);
            _groups = _ids.Values.ToDictionary(x => x.Group, x => x);
        }

        public VoidHuntersEntityDescriptor GetById(Id<VoidHuntersEntityDescriptor> id)
        {
            return _ids[id];
        }

        public VoidHuntersEntityDescriptor GetByGroup(ExclusiveGroupStruct group)
        {
            return _groups[group];
        }

        public IEnumerable<VoidHuntersEntityDescriptor> GetAll()
        {
            return _ids.Values;
        }
    }
}
