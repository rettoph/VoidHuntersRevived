using Svelto.ECS;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Utilities
{
    public static class EntityTypeHelper
    {
        public class StaticEntityData
        {
            public EGID EGID { get; }
            public TypeData StaticComponent { get; }
            public InstanceData InstanceComponent { get; set; }

            public StaticEntityData(EGID eGID, TypeData staticComponent, InstanceData instanceComponent)
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

            ExclusiveGroupStruct group = ExclusiveGroupStructHelper.GetOrCreateExclusiveStruct($"{entityType.Descriptor.GetType().Name}.Static");
            EGID egid = new EGID(_id++, group);
            CombinedFilterID filter = new CombinedFilterID((int)egid.entityID, TypeData.InstanceEntitiesFilterContextId);

            data = new StaticEntityData(egid, new TypeData(filter), new InstanceData(default));

            return data;
        }

        public static void SetGroupIndex(IEntityType entityType, GroupIndex groupIndex)
        {
            StaticEntityData data = EntityTypeHelper.GetData(entityType);

            data.InstanceComponent = new InstanceData(groupIndex);
        }
    }
}
