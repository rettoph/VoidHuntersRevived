using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Simulations.Engines
{
    [AutoLoad]
    [SequenceGroup<EngineSequence>(EngineSequence.Group03)]
    [SequenceGroup<DrawSequence>(DrawSequence.Draw)]
    internal class DrawGameTimeEnginesEngine : StrategyEngine, IStepEngine<Frame>
    {
        private IStepGroupEngine<GameTime> _drawEnginesGroup = null!;

        public string name => nameof(DrawGameTimeEnginesEngine);

        public override void Initialize(IStrategy simulation)
        {
            base.Initialize(simulation);

            _drawEnginesGroup = simulation.Engines.All().CreateSequencedStepEnginesGroup<GameTime, DrawSequence>(DrawSequence.Draw);
        }

        public void Step(in Frame param)
        {
            _drawEnginesGroup.Step(param.GameTime);
        }
    }
}
