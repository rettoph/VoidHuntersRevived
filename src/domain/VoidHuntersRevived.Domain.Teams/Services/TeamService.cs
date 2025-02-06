using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Teams.Services
{
    public class TeamService(
        IEntityTemplateService entityTemplateService,
        IPrivateEntitySpawnService privateEntitySpawnService
    ) : StrategySystem, ITeamService, IOnInitializeEngine
    {
        private Team _defaultTeamComponent;
        private readonly Dictionary<Id<Team>, Team> _teamComponents = [];

        private readonly IEntityTemplateService _entityTemplateService = entityTemplateService;
        private readonly IPrivateEntitySpawnService _privateEntitySpawnService = privateEntitySpawnService;

        [SequenceGroup<OnInitializeSequenceGroupEnum>(OnInitializeSequenceGroupEnum.Initialize)]
        public void OnInitialize(IStrategy strategy)
        {
            this.BuildTeams();
        }

        public Team GetDefaultTeam()
        {
            return this._defaultTeamComponent;
        }

        public Team GetOpenTeam()
        {
            return this._teamComponents.First().Value;
        }

        private void BuildTeams()
        {
            // Spawn default team entities...
            int teamIndex = 0;
            foreach (IEntityTemplate teamEntityTemplate in this._entityTemplateService.WithComponent<Team>())
            {
                teamIndex++;
                EntityLocalId teamLocalId = this._privateEntitySpawnService.Spawn(
                    sourceId: HashBuilder<Team, int>.Instance.Calculate(teamIndex),
                    entityTemplateKey: teamEntityTemplate.Key,
                    globalId: HashBuilder<Team, int>.Instance.Calculate(teamIndex).ToGlobalEntityId(),
                    initializer: this.TeamInstanceInitializer);
            }
        }

        private void TeamInstanceInitializer(IEntityService entities, in InitializingEntity entity)
        {
            Team importedTeam = entity.Initializer.Get<Team>();
            Team runtimeTeam = new(entity.LocalId, importedTeam.Name);
            entity.Initializer.Init<Team>(runtimeTeam);

            if (entity.Initializer.Has<DefaultTeam>())
            {
                this._defaultTeamComponent = runtimeTeam;
            }
            else
            {
                this._teamComponents.Add(runtimeTeam.Id, runtimeTeam);
            }
        }
    }
}