using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Engines
{
    [AutoLoad]
    internal class ColorSchemeEngine : BasicEngine, IReactOnAddEx<ColorScheme>
    {
        private readonly IEntityService _entities;

        public ColorSchemeEngine(IEntityService entities)
        {
            _entities = entities;
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<ColorScheme> entities, ExclusiveGroupStruct groupID)
        {
            var (instanceDatas, colorSchemes, teamGroupIds, _) = _entities.QueryEntities<InstanceData, ColorScheme, GroupIndex<Team>>(groupID);

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                ref InstanceData instanceData = ref instanceDatas[i];
                ref ColorScheme instanceColorScheme = ref colorSchemes[i];
                ref GroupIndex<Team> teamGroupId = ref teamGroupIds[i];
                ref ColorScheme teamColorScheme = ref _entities.QueryByGroupIndex<ColorScheme>(teamGroupId.Value);

            }
        }
    }
}
