using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityDescriptorService : IEntityDescriptorService
    {
        private Dictionary<Guid, VoidHuntersEntityDescriptor> _descriptors;

        public EntityDescriptorService(IEnumerable<VoidHuntersEntityDescriptor> descriptors)
        {
            _descriptors = descriptors.ToDictionary(x => x.Id.Value, x => x);
        }

        public VoidHuntersEntityDescriptor GetById(VhId id)
        {
            return _descriptors[id.Value];
        }
    }
}
