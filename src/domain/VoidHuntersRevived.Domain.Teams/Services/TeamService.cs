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
    internal class TeamService : BasicEngine, ITeamService
    {
        private BelongsTo<Team, TeamMember> _belongsToDefaultTeamComponent;
        private Dictionary<Id<Team>, GroupIndex> _groupIndices;


        private readonly IEntityService _entities;
        private readonly IEntityTypeService _types;

        public TeamService(IEntityService entities, IEntityTypeService types)
        {
            _entities = entities;
            _groupIndices = new Dictionary<Id<Team>, GroupIndex>();
            _types = types;
        }

        public unsafe override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);

            foreach (var ((teams, entityIds, count), group) in _entities.QueryEntities<Team, EntityId>())
            {
                for (uint i = 0; i < count; i++)
                {
                    var team = teams[i];
                    _groupIndices.Add(team.Id, new GroupIndex(group, i));
                    _belongsToDefaultTeamComponent = new BelongsTo<Team, TeamMember>(entityIds[i]);
                }
            }
        }

        public bool TryGetGroupIndex(Id<Team> teamId, out GroupIndex groupIndex)
        {
            return _groupIndices.TryGetValue(teamId, out groupIndex);
        }

        public BelongsTo<Team, TeamMember> GetDefaultTeamComponent()
        {
            return _belongsToDefaultTeamComponent;
        }

        public BelongsTo<Team, TeamMember> GetOpenTeamComponent()
        {
            return _belongsToDefaultTeamComponent;
        }
    }
}
