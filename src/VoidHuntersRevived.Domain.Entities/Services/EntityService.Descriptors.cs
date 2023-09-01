using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common;
using Guppy.Common.Collections;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService
    {
        private readonly DoubleDictionary<VhId, Type, IVoidHuntersEntityDescriptorEngine> _descriptors;

        private IVoidHuntersEntityDescriptorEngine GetDescriptorEngine(VhId descriptorId)
        {
            return _descriptors[descriptorId];
        }

        IVoidHuntersEntityDescriptorEngine IEntityService.GetDescriptorEngine(VhId descriptorId)
        {
            return _descriptors[descriptorId];
        }
    }
}
