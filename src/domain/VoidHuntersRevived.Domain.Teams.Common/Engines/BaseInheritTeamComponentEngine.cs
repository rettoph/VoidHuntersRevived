using Svelto.ECS;
using VoidHuntersRevived.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Engines
{
    /// <summary>
    /// Allows for entity components to be overwritten by their teams
    /// </summary>
    public abstract class BaseInheritTeamComponentEngine<TComponent>(IEntityQueryService entityQueryService) : StrategyEngine, IReactOnAddEx<TComponent>
        where TComponent : unmanaged, IEntityComponent
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<TComponent> entities, ExclusiveGroupStruct groupID)
        {
            if (_entityQueryService.HasAll<EntityTemplate, TeamMember>(groupID, out var components) == false)
            {
                return;
            }

            var (instanceComponents, _) = entities;
            var (_, teamMembers, _) = components;

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                ref TComponent instanceComponent = ref instanceComponents[i];

                ref TeamMember teamMember = ref teamMembers[i];
                if (_entityQueryService.TryQueryByVhId<TComponent>(teamMember.OwnerFilterId.VhId, out TComponent teamComponent) && teamComponent.IsDefault() == false)
                {
                    instanceComponent = teamComponent;
                    continue;
                }
            }
        }
    }
}
