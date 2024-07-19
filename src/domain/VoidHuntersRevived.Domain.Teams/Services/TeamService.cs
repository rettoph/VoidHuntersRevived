using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Teams.Services
{
    internal class TeamService : StrategyEngine, ITeamService
    {
        private struct TeamData
        {
            public required Id<Team> Id { get; init; }
            public required GroupIndex GroupIndex { get; init; }
            public required BelongsTo<Team, TeamMember> Component { get; init; }
        }

        private BelongsTo<Team, TeamMember> _defaultTeamComponent;
        private Dictionary<Id<Team>, TeamData> _teams;


        private readonly IEntityQueryService _entityQueryService;

        public TeamService(IEntityQueryService entityQueryService)
        {
            _entityQueryService = entityQueryService;
            _teams = new Dictionary<Id<Team>, TeamData>();
        }

        public unsafe override void Initialize(IStrategy strategy)
        {
            base.Initialize(strategy);


            _defaultTeamComponent = this.BuildDeaultTeamComponent();

            this.BuildTeams(_teams);
        }

        public bool TryGetGroupIndex(Id<Team> teamId, out GroupIndex groupIndex)
        {
            if (_teams.TryGetValue(teamId, out TeamData data) == false)
            {
                groupIndex = default;
                return false;
            }

            groupIndex = data.GroupIndex;
            return true;
        }

        public BelongsTo<Team, TeamMember> GetDefaultTeamComponent()
        {
            return _defaultTeamComponent;
        }

        public BelongsTo<Team, TeamMember> GetOpenTeamComponent()
        {
            return _teams.First().Value.Component;
        }

        private BelongsTo<Team, TeamMember> BuildDeaultTeamComponent()
        {
            foreach (var ((teams, entityIds, _, count), group) in _entityQueryService.QueryEntities<Team, EntityId, DefaultTeam>())
            {
                if (count == 0)
                {
                    throw new NotImplementedException();
                }

                if (count >= 2)
                {
                    throw new NotImplementedException();
                }

                var team = teams[0];
                return new BelongsTo<Team, TeamMember>(entityIds[0].VhId);
            }

            throw new NotImplementedException();
        }

        private void BuildTeams(Dictionary<Id<Team>, TeamData> dictionary)
        {
            foreach (var ((teams, entityIds, _, count), group) in _entityQueryService.QueryEntities<Team, EntityId, ColorScheme>())
            {
                for (uint i = 0; i < count; i++)
                {
                    var team = teams[i];
                    dictionary.Add(team.Id, new TeamData()
                    {
                        Id = team.Id,
                        GroupIndex = new GroupIndex(group, i),
                        Component = new BelongsTo<Team, TeamMember>(entityIds[i].VhId)
                    });
                }
            }
        }
    }
}
