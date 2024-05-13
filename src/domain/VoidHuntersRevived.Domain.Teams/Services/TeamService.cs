using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Teams.Services
{
    internal class TeamService : BasicEngine, ITeamService
    {
        private Id<Team> _defaultId;
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

            foreach (var ((teams, count), group) in _entities.QueryEntities<Team>())
            {
                for (uint i = 0; i < count; i++)
                {
                    var team = teams[i];
                    _groupIndices.Add(team.Id, new GroupIndex(group, i));
                    _defaultId = team.Id;
                }
            }
        }

        public bool TryGetGroupIndex(Id<Team> teamId, out GroupIndex groupIndex)
        {
            return _groupIndices.TryGetValue(teamId, out groupIndex);
        }

        public Id<Team> GetDefaultTeamId()
        {
            return _defaultId;
        }

        public Id<Team> GetOpenTeamId()
        {
            return _defaultId;
        }
    }
}
