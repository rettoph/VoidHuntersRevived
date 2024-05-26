using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityDescriptorService
    {
        VoidHuntersEntityDescriptor GetById(Id<VoidHuntersEntityDescriptor> id);

        VoidHuntersEntityDescriptor GetByGroup(ExclusiveGroupStruct group);

        IEnumerable<VoidHuntersEntityDescriptor> GetAll();
    }
}
