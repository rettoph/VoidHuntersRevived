using Guppy.Attributes;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;

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
