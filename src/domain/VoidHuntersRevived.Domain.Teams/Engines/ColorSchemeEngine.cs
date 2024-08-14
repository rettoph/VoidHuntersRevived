using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Engines
{
    /// <summary>
    /// Responsible for setting a new instance entity's color scheme.
    /// It will first use the teams color scheme, if any.
    /// Otherwise it will default to the entity type color scheme.
    /// 
    ///   1. Team component value
    ///   2. Instance entity's Type component value
    ///   3. Reset component value
    /// 
    /// </summary>
    [AutoLoad]
    internal class ColorSchemeEngine : StrategyEngine, IReactOnAddEx<ColorScheme>
    {
        private readonly IEntityQueryService _entityQueryService;

        public ColorSchemeEngine(IEntityQueryService entityQueryService)
        {
            _entityQueryService = entityQueryService;
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<ColorScheme> entities, ExclusiveGroupStruct groupID)
        {
            if (_entityQueryService.HasAll<InstanceEntity, BelongsTo<Team, TeamMember>, BelongsTo<TypeEntity, InstanceEntity>>(groupID, out var components) == false)
            {
                return;
            }

            var (instanceColorSchemes, _) = entities;
            var (instances, belongsToTeams, belongsToTypes, _) = components;

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                ref ColorScheme instanceColorScheme = ref instanceColorSchemes[i];

                ref BelongsTo<Team, TeamMember> belongsToTeam = ref belongsToTeams[i];
                if (_entityQueryService.TryQueryByVhId<ColorScheme>(belongsToTeam.OwnerVhId, out ColorScheme teamColorScheme) && teamColorScheme.IsDefault() == false)
                {
                    instanceColorScheme = teamColorScheme;
                    continue;
                }

                ref BelongsTo<TypeEntity, InstanceEntity> belongsToType = ref belongsToTypes[i];
                if (_entityQueryService.TryQueryByVhId<ColorScheme>(belongsToTypes[i].OwnerVhId, out ColorScheme typeColorScheme))
                {
                    instanceColorScheme = typeColorScheme;
                    continue;
                }
            }
        }
    }
}
