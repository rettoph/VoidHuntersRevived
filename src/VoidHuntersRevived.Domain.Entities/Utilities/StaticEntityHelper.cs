using Svelto.ECS;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Utilities
{
    public static class StaticEntityHelper
    {
        private static uint _id = 0;
        private static readonly Dictionary<IEntityType, (EGID, StaticEntity, InstanceEntity)> _components = new Dictionary<IEntityType, (EGID, StaticEntity, InstanceEntity)>();

        public static (EGID, StaticEntity, InstanceEntity) GetComponents(IEntityType entityType)
        {
            ref (EGID egid, StaticEntity @static, InstanceEntity instance) components = ref CollectionsMarshal.GetValueRefOrAddDefault(_components, entityType, out bool exists);
            if (exists)
            {
                return components;
            }

            ExclusiveGroupStruct group = ExclusiveGroupStructHelper.GetOrCreateExclusiveStruct($"{entityType.GetType().Name}.Static");
            EGID egid = new EGID(_id++, group);
            CombinedFilterID filter = new CombinedFilterID((int)egid.entityID, StaticEntity.InstanceEntitiesFilterContextId);

            components.egid = egid;
            components.@static = new StaticEntity(filter);
            components.instance = new InstanceEntity(egid, filter);

            return components;
        }
    }
}
