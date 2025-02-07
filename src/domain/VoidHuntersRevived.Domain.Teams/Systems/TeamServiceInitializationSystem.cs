using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Domain.Teams.Services;

namespace VoidHuntersRevived.Domain.Teams.Systems
{
    public class TeamServiceInitializationSystem(TeamService teamService) : ISceneSystem, IInitializeSystem
    {
        private readonly TeamService _teamService = teamService;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Initialize)]
        public void Initialize()
        {
            this._teamService.Initialize();
        }
    }
}
