using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Engines
{
    [AutoLoad]
    internal sealed class TeamIdEngine : BasicEngine, IReactOnAddEx<Id<Team>>
    {
        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Id<Team>> entities, ExclusiveGroupStruct groupID)
        {
            var (teamIds, _) = entities;
            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                if (teamIds[i] == default)
                {
                    throw new Exception($"No team defined. Ensure an {nameof(Id<Team>)} component gets initialized to the entity.");
                }
            }
        }
    }
}
