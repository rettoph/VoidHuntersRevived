using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityDescriptorService
    {
        VoidHuntersEntityDescriptor GetById(Id<VoidHuntersEntityDescriptor> id);

        VoidHuntersEntityDescriptor GetByGroup(ExclusiveGroupStruct group);

        IEnumerable<VoidHuntersEntityDescriptor> GetAll();
    }
}
