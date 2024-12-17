using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Teams.Services
{
    public class TeamService(
        IEntityTemplateService entityTemplateService,
        IEntityQueryService entityQueryService,
        IPrivateEntitySpawnService privateEntitySpawnService) : StrategyEngine, ITeamService, IOnInitializeEngine
    {
        private Team _defaultTeamComponent;
        private readonly Dictionary<Id<Team>, Team> _teamComponents = [];

        private readonly IEntityTemplateService _entityTemplateService = entityTemplateService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IPrivateEntitySpawnService _privateEntitySpawnService = privateEntitySpawnService;

        [SequenceGroup<OnInitializeSequenceGroup>(OnInitializeSequenceGroup.Initialize)]
        public void OnInitialize(IStrategy strategy)
        {
            this.BuildTeams();
        }

        public Team GetDefaultTeam()
        {
            return _defaultTeamComponent;
        }

        public Team GetOpenTeam()
        {
            return _teamComponents.First().Value;
        }

        private void BuildTeams()
        {
            // Spawn default team entities...
            int teamIndex = 0;
            foreach (IEntityTemplate teamEntityTemplate in _entityTemplateService.WithComponent<Team>())
            {
                teamIndex++;
                EntityLocalId teamLocalId = _privateEntitySpawnService.Spawn(
                    sourceId: HashBuilder<Team, int>.Instance.Calculate(teamIndex),
                    entityTemplateKey: teamEntityTemplate.Key,
                    globalId: HashBuilder<Team, int>.Instance.Calculate(teamIndex).ToGlobalEntityId(),
                    initializer: TeamInstanceInitializer);
            }
        }

        private void TeamInstanceInitializer(IEntityService entities, in InitializingEntity entity)
        {
            Team importedTeam = entity.Initializer.Get<Team>();
            Team runtimeTeam = new(entity.LocalId, importedTeam.Name);
            entity.Initializer.Init<Team>(runtimeTeam);

            if (entity.Initializer.Has<DefaultTeam>())
            {
                _defaultTeamComponent = runtimeTeam;
            }
            else
            {
                _teamComponents.Add(runtimeTeam.Id, runtimeTeam);
            }
        }
    }
}
