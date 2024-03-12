using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Teams;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Teams.Engines
{
    [AutoLoad]
    internal sealed class TeamIdEngine : BasicEngine, IReactOnAddEx<Id<ITeam>>
    {
        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Id<ITeam>> entities, ExclusiveGroupStruct groupID)
        {
            var (teamIds, _) = entities;
            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                if (teamIds[i] == default)
                {
                    throw new Exception($"No team defined. Ensure an Id<Iteam> component gets initialized onto the entity.");
                }
            }
        }
    }
}
