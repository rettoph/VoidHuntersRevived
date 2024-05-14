using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Extensions;
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
            if(_entities.HasAll<InstanceData, GroupIndex<Team>>(groupID, out var components) == false)
            {
                return;
            }

            var (colorSchemes, _) = entities;
            var (instanceDatas, teamGroupIds, _) = components;

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                ref InstanceData instanceData = ref instanceDatas[i];
                ref ColorScheme colorScheme = ref colorSchemes[i];
                ref GroupIndex<Team> teamGroupId = ref teamGroupIds[i];

                if(_entities.TryQueryByGroupIndex<ColorScheme>(teamGroupId.Value, out ColorScheme teamColorScheme) && teamColorScheme.IsDefault() == false)
                {
                    colorScheme = teamColorScheme;
                    return;
                }

                if(_entities.TryQueryByGroupIndex<ColorScheme>(instanceData.StaticEntityId, out ColorScheme staticColorScheme))
                {
                    colorScheme = staticColorScheme;
                    return;
                }
            }
        }
    }
}
