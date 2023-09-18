using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common;
using Guppy.Common.Collections;
using VoidHuntersRevived.Common.Entities;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService
    {
        private readonly DoubleDictionary<Id<IEntityComponent>, Type, IVoidHuntersEntityDescriptorEngine> _descriptors;

        private IVoidHuntersEntityDescriptorEngine GetDescriptorEngine(Id<IEntityComponent> descriptorId)
        {
            return _descriptors[descriptorId];
        }

        IVoidHuntersEntityDescriptorEngine IEntityService.GetDescriptorEngine(Id<IEntityComponent> descriptorId)
        {
            return _descriptors[descriptorId];
        }
    }
}
