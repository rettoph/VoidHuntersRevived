using Guppy.Common.Collections;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService
    {
        private readonly DoubleDictionary<Id<VoidHuntersEntityDescriptor>, Type, IVoidHuntersEntityDescriptorEngine> _descriptors;

        private IVoidHuntersEntityDescriptorEngine GetDescriptorEngine(Id<VoidHuntersEntityDescriptor> descriptorId)
        {
            return _descriptors[descriptorId];
        }

        IVoidHuntersEntityDescriptorEngine IEntityService.GetDescriptorEngine(Id<VoidHuntersEntityDescriptor> descriptorId)
        {
            return _descriptors[descriptorId];
        }
    }
}
