using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Simulations.Engines
{
    [AutoLoad]
    [Sequence<DrawSequence>(DrawSequence.Draw)]
    internal class DrawGameTimeEnginesEngine : BasicEngine, IStepEngine<Frame>, IEngineEngine
    {
        private IStepGroupEngine<GameTime> _drawEnginesGroup = null!;

        public string name => nameof(DrawGameTimeEnginesEngine);

        public void Initialize(IEngine[] engines)
        {
            _drawEnginesGroup = engines.CreateSequencedStepEnginesGroup<GameTime, DrawSequence>(DrawSequence.Draw);
        }

        public void Step(in Frame param)
        {
            _drawEnginesGroup.Step(param.GameTime);
        }
    }
}
