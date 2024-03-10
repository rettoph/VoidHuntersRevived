using Guppy.Attributes;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Simulations.Engines
{
    [AutoLoad]
    internal class DrawGameTimeTeamEnginesEngine : BasicEngine, IStepEngine<GameTime>, IEngineEngine
    {
        private IStepGroupEngine<GameTimeTeam> _teamDrawEnginesGroup = null!;
        private readonly ITeamService _teams;

        public DrawGameTimeTeamEnginesEngine(ITeamService teams)
        {
            _teams = teams;
        }

        public string name { get; } = nameof(DrawGameTimeTeamEnginesEngine);

        public void Initialize(IEngine[] engines)
        {
            _teamDrawEnginesGroup = engines.CreateSequencedStepEnginesGroup<GameTimeTeam, DrawSequence>(DrawSequence.Draw);
        }

        public void Step(in GameTime param)
        {
            foreach (ITeam team in _teams.GetAll())
            {
                _teamDrawEnginesGroup.Step(new GameTimeTeam(param, team));
            }
        }
    }
}
