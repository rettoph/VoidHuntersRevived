using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Teams.Services
{
    internal class TeamService(
        IEntityTemplateService entityTemplateService,
        IEntityQueryService entityQueryService,
        IPrivateEntitySpawnService privateEntitySpawnService) : StrategyEngine, ITeamService, IOnInitializeEngine
    {
        private readonly struct TeamData
        {
            public required Id<Team> Id { get; init; }
            public required GroupIndex GroupIndex { get; init; }
            public required BelongsTo<Team, TeamMember> Component { get; init; }
        }

        private BelongsTo<Team, TeamMember> _defaultTeamComponent;
        private Dictionary<Id<Team>, BelongsTo<Team, TeamMember>> _teamComponents = [];

        private readonly IEntityTemplateService _entityTemplateService = entityTemplateService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IPrivateEntitySpawnService _privateEntitySpawnService = privateEntitySpawnService;

        [SequenceGroup<OnInitializeSequenceGroup>(OnInitializeSequenceGroup.Initialize)]
        public void OnInitialize(IStrategy strategy)
        {
            this.BuildTeams(out _defaultTeamComponent, out _teamComponents);
        }

        public BelongsTo<Team, TeamMember> GetDefaultTeamComponent()
        {
            return _defaultTeamComponent;
        }

        public BelongsTo<Team, TeamMember> GetOpenTeamComponent()
        {
            return _teamComponents.First().Value;
        }

        private void BuildTeams(out BelongsTo<Team, TeamMember> defaultTeamComponent, out Dictionary<Id<Team>, BelongsTo<Team, TeamMember>> teamComponents)
        {
            // Spawn default team entity...
            int teamIndex = 0;
            IEntityTemplate defaultTeamTemplate = _entityTemplateService.WithComponent<DefaultTeam>().Single();
            EntityId defaultTeamId = _privateEntitySpawnService.Spawn(
                sourceId: HashBuilder<DefaultTeam, int>.Instance.Calculate(teamIndex),
                entityTemplateKey: defaultTeamTemplate.Key,
                vhid: HashBuilder<DefaultTeam, int>.Instance.Calculate(teamIndex));
            defaultTeamComponent = new BelongsTo<Team, TeamMember>(defaultTeamId.VhId);

            // Spawn additional team entities...
            teamComponents = [];
            IEntityTemplate[] teamEntityTemplates = _entityTemplateService.WithComponent<Team>()
                .Except([defaultTeamTemplate]).ToArray();

            foreach (IEntityTemplate teamEntityTemplate in teamEntityTemplates)
            {
                teamIndex++;
                EntityId teamId = _privateEntitySpawnService.Spawn(
                    sourceId: HashBuilder<Team, int>.Instance.Calculate(teamIndex),
                    entityTemplateKey: teamEntityTemplate.Key,
                    vhid: HashBuilder<Team, int>.Instance.Calculate(teamIndex));

                teamComponents.Add(teamEntityTemplate.Components.Get<Team>().Id, new BelongsTo<Team, TeamMember>(teamId.VhId));
            }
        }
    }
}
