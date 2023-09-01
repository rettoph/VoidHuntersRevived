﻿using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IScopedEntityDescriptorService
    {
        ScopedVoidHuntersEntityDescriptor GetById(VhId id);

        ScopedVoidHuntersEntityDescriptor GetByEntityVhId(VhId id);

        EntityInitializer Spawn(VoidHuntersEntityDescriptor descriptor, IEntityFactory factory, VhId vhid, out EntityId id);

        void SoftDespawn(IEntityService entities, in EntityId id);

        void HardDespawn(IEntityService entities, IEntityFunctions functions, in EntityId id);
    }
}
