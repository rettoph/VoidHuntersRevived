using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Engines
{
    [AutoLoad]
    internal class zIndexEngine : StrategyEngine, IReactOnAddEx<zIndex>
    {
        private readonly IEntityQueryService _entityQueryService;

        public zIndexEngine(IEntityQueryService entityQueryService)
        {
            _entityQueryService = entityQueryService;
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<zIndex> entities, ExclusiveGroupStruct groupID)
        {
            if (_entityQueryService.HasAll<InstanceEntity, BelongsTo<Team, TeamMember>, BelongsTo<TypeEntity, InstanceEntity>>(groupID, out var components) == false)
            {
                return;
            }

            var (instanceComponents, _) = entities;
            var (instances, belongsToTeams, belongsToTypes, _) = components;

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                ref zIndex instanceComponent = ref instanceComponents[i];

                ref BelongsTo<Team, TeamMember> belongsToTeam = ref belongsToTeams[i];
                if (_entityQueryService.TryQueryByVhId<zIndex>(belongsToTeam.OwnerVhId, out zIndex teamComponent) && teamComponent.IsDefault() == false)
                {
                    instanceComponent = teamComponent;
                    continue;
                }

                ref BelongsTo<TypeEntity, InstanceEntity> belongsToType = ref belongsToTypes[i];
                if (_entityQueryService.TryQueryByVhId<zIndex>(belongsToTypes[i].OwnerVhId, out zIndex typeComponent))
                {
                    instanceComponent = typeComponent;
                    continue;
                }

                // No valid team or type value, reset to default
                instanceComponent = default;
            }
        }
    }
}
