using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.ECS.Services
{
    public interface IEntityService
    {
        EntityId Create(EntityType type, EntityId id);
        EntityId Create(EntityType type, EntityId id, Action<IEntityInitializer> initializer);

        void Destroy(EntityId entityKey);
    }
}
