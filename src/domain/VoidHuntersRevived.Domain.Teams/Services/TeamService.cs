using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.EntityTypes;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Teams.Services
{
    [Sequence<EngineSequence>(EngineSequence.Group03)]
    internal class TeamService : StrategyEngine, ITeamService
    {
        private struct TeamData
        {
            public required Id<Team> Id { get; init; }
            public required GroupIndex GroupIndex { get; init; }
            public required BelongsTo<Team, TeamMember> Component { get; init; }
        }

        private BelongsTo<Team, TeamMember> _defaultTeamComponent;
        private Dictionary<Id<Team>, BelongsTo<Team, TeamMember>> _teamComponents;

        private readonly IEntityTypeService _entityTypeService;
        private readonly IEntityQueryService _entityQueryService;
        private readonly IPrivateEntitySpawnService _privateEntitySpawnService;

        public TeamService(
            IEntityTypeService entityTypeService,
            IEntityQueryService entityQueryService,
            IPrivateEntitySpawnService privateEntitySpawnService)
        {
            _entityTypeService = entityTypeService;
            _entityQueryService = entityQueryService;
            _privateEntitySpawnService = privateEntitySpawnService;
            _teamComponents = new Dictionary<Id<Team>, BelongsTo<Team, TeamMember>>();
        }

        public unsafe override void Initialize(IStrategy strategy)
        {
            base.Initialize(strategy);

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
            DefaultTeamEntityType defaultTeamType = _entityTypeService.GetAll<DefaultTeamEntityType>().Single();
            EntityId defaultTeamId = _privateEntitySpawnService.Spawn(
                sourceId: HashBuilder<DefaultTeamEntityType, int>.Instance.Calculate(teamIndex),
                entityTypeKey: defaultTeamType.Key,
                vhid: HashBuilder<TeamEntityType, int>.Instance.Calculate(teamIndex));
            defaultTeamComponent = new BelongsTo<Team, TeamMember>(defaultTeamId.VhId);

            // Spawn additional team entities...
            teamComponents = new Dictionary<Id<Team>, BelongsTo<Team, TeamMember>>();
            TeamEntityType[] teamEntityTypes = _entityTypeService.GetAll<TeamEntityType>();

            foreach (IEntityType teamEntityType in teamEntityTypes)
            {
                teamIndex++;
                EntityId teamId = _privateEntitySpawnService.Spawn(
                    sourceId: HashBuilder<DefaultTeamEntityType, int>.Instance.Calculate(teamIndex),
                    entityTypeKey: teamEntityType.Key,
                    vhid: HashBuilder<TeamEntityType, int>.Instance.Calculate(teamIndex));

                teamComponents.Add(teamEntityType.Components.Get<Team>().Id, new BelongsTo<Team, TeamMember>(teamId.VhId));
            }
        }
    }
}
