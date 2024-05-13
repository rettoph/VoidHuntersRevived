using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Descriptors;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Teams.Engines
{
    [AutoLoad]
    [Sequence<EngineSequence>(EngineSequence.PreInitialize)]
    internal sealed class TeamIdEngine : BasicEngine, IReactOnAddEx<Instance<TeamMemberEntityDescriptor>>
    {
        private readonly IEntityService _entities;
        private readonly ITeamService _teams;

        public TeamIdEngine(IEntityService entities, ITeamService teams)
        {
            _entities = entities;
            _teams = teams;
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Instance<TeamMemberEntityDescriptor>> entities, ExclusiveGroupStruct groupID)
        {
            var (teamIds, groupIds, _) = _entities.QueryEntities<Id<Team>, GroupIndex<Team>>(groupID);

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
