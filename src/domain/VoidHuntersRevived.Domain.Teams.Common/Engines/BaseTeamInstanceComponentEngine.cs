using Svelto.ECS;
using VoidHuntersRevived.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Engines
{
    /// <summary>
    /// Responsible for setting an instance entity's component to it's
    /// Teams value. Allows for instances to inherit values from their
    /// current team.
    /// 
    ///   1. Team component value
    ///   2. Instance entity's Type component value
    ///   3. Reset component value
    /// 
    /// </summary>
    public abstract class BaseTeamInstanceComponentEngine<TComponent> : StrategyEngine, IReactOnAddEx<TComponent>
        where TComponent : unmanaged, IEntityComponent
    {
        private readonly IEntityQueryService _entityQueryService;

        public BaseTeamInstanceComponentEngine(IEntityQueryService entityQueryService)
        {
            _entityQueryService = entityQueryService;
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<TComponent> entities, ExclusiveGroupStruct groupID)
        {
            if (_entityQueryService.HasAll<InstanceEntity, BelongsTo<Team, TeamMember>, BelongsTo<TypeEntity, InstanceEntity>>(groupID, out var components) == false)
            {
                return;
            }

            var (instanceComponents, _) = entities;
            var (instances, belongsToTeams, belongsToTypes, _) = components;

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                ref TComponent instanceComponent = ref instanceComponents[i];

                ref BelongsTo<Team, TeamMember> belongsToTeam = ref belongsToTeams[i];
                if (_entityQueryService.TryQueryByVhId<TComponent>(belongsToTeam.OwnerVhId, out TComponent teamComponent) && teamComponent.IsDefault() == false)
                {
                    instanceComponent = teamComponent;
                    continue;
                }

                ref BelongsTo<TypeEntity, InstanceEntity> belongsToType = ref belongsToTypes[i];
                if (_entityQueryService.TryQueryByVhId<TComponent>(belongsToTypes[i].OwnerVhId, out TComponent typeComponent))
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
