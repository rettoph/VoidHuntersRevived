using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Teams.Engines
{
    [AutoLoad]
    [Sequence<EngineSequence>(EngineSequence.Group00)]
    internal sealed class TeamGroupIndexEngine : BasicEngine, IReactOnAddEx<Id<Team>>
    {
        private readonly IEntityService _entities;
        private readonly ITeamService _teams;

        public TeamGroupIndexEngine(IEntityService entities, ITeamService teams)
        {
            _entities = entities;
            _teams = teams;
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Id<Team>> entities, ExclusiveGroupStruct groupID)
        {
            var (teamIds, _) = entities;
            var (groupIds, _) = _entities.TryQueryEntities<GroupIndex<Team>>(groupID, out bool success);
            if(success == false)
            {
                return;
            }

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                ref Id<Team> teamId = ref teamIds[i];
                if (teamId == default)
                {
                    throw new Exception($"No team defined. Ensure an {nameof(Id<Team>)} component gets initialized to the entity.");
                }

                if (_teams.TryGetGroupIndex(teamId, out GroupIndex teamGroupIndexValue) == false)
                {
                    throw new InvalidOperationException($"Unknown {nameof(Id<Team>)} - {teamId}");
                }

                ref GroupIndex<Team> groupId = ref groupIds[0];
                groupId.Value = teamGroupIndexValue;
            }
        }
    }
}
