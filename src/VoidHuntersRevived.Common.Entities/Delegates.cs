using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities
{
    public delegate void EntityInitializerDelegate(IEntityService entities, ref EntityInitializer initializer, in EntityId id);
}
