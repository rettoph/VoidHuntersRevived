using Svelto.ECS;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Utilities
{
    public static class StaticEntityHelper
    {
        public class StaticEntityData
        {
            public EGID EGID { get; }
            public StaticEntity StaticComponent { get; }
            public InstanceEntity InstanceComponent { get; set; }

            public StaticEntityData(EGID eGID, StaticEntity staticComponent, InstanceEntity instanceComponent)
            {
                this.EGID = eGID;
                this.StaticComponent = staticComponent;
                this.InstanceComponent = instanceComponent;
            }
        }

        private static uint _id = 0;
        private static readonly Dictionary<IEntityType, StaticEntityData> _data = new Dictionary<IEntityType, StaticEntityData>();

        public static StaticEntityData GetData(IEntityType entityType)
        {
            ref StaticEntityData? data = ref CollectionsMarshal.GetValueRefOrAddDefault(_data, entityType, out bool exists);
            if (exists)
            {
                return data!;
            }

            ExclusiveGroupStruct group = ExclusiveGroupStructHelper.GetOrCreateExclusiveStruct($"{entityType.GetType().Name}.Static");
            EGID egid = new EGID(_id++, group);
            CombinedFilterID filter = new CombinedFilterID((int)egid.entityID, StaticEntity.InstanceEntitiesFilterContextId);

            data = new StaticEntityData(egid, new StaticEntity(filter), new InstanceEntity(default, filter));

            return data;
        }

        public static void SetGroupIndex(IEntityType entityType, GroupIndex groupIndex)
        {
            StaticEntityData data = StaticEntityHelper.GetData(entityType);

            data.InstanceComponent = new InstanceEntity(groupIndex, data.InstanceComponent.FilterId);
        }
    }
}
