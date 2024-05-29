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
    /// Responsible for setting an instance entity's color scheme.
    /// Scheme selection is done in the following priority:
    /// 
    ///   1. Team Color Scheme
    ///   2. Static Color Scheme
    /// 
    /// </summary>
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
            if (_entities.HasAll<InstanceEntity, BelongsTo<Team, TeamMember>, BelongsTo<TypeEntity, InstanceEntity>>(groupID, out var components) == false)
            {
                return;
            }

            var (colorSchemes, _) = entities;
            var (instances, belongsToTeams, belongsToTypes, _) = components;

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                ref ColorScheme colorScheme = ref colorSchemes[i];

                ref BelongsTo<Team, TeamMember> belongsToTeam = ref belongsToTeams[i];
                if (_entities.TryQueryById<ColorScheme>(belongsToTeam.OwnerId, out ColorScheme teamColorScheme) && teamColorScheme.IsDefault() == false)
                {
                    colorScheme = teamColorScheme;
                    continue;
                }

                ref BelongsTo<TypeEntity, InstanceEntity> belongsToType = ref belongsToTypes[i];
                if (_entities.TryQueryById<ColorScheme>(belongsToTypes[i].OwnerId, out ColorScheme typeColorScheme))
                {
                    colorScheme = typeColorScheme;
                    continue;
                }
            }
        }
    }
}
