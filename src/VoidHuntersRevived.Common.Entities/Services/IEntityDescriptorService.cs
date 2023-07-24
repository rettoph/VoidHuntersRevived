using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityDescriptorService
    {
        VoidHuntersEntityDescriptor GetById(VhId id);

        VoidHuntersEntityDescriptor GetByEntityVhId(VhId id);

        EntityInitializer Spawn(VoidHuntersEntityDescriptor descriptor, IEntityFactory factory, VhId vhid, out EntityId id);

        void Despawn(IEntityFunctions functions, in EntityId id);
    }
}
