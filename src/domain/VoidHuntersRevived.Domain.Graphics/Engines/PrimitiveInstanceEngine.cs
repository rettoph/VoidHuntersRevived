using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Graphics.Engines
{
    [AutoLoad]
    [Sequence<EngineSequence>(EngineSequence.Group01)]
    internal sealed class PrimitiveInstanceEngine : StrategyEngine
    {
        private readonly IPrivateEntitySpawnService _privateEntitySpawnService;
        private readonly IPrimitiveService _primitiveService;

        public PrimitiveInstanceEngine(
            IPrivateEntitySpawnService privateEntitySpawnService,
            IPrimitiveService primitiveService)
        {
            _privateEntitySpawnService = privateEntitySpawnService;
            _primitiveService = primitiveService;
        }

        public override void Initialize(IStrategy strategy)
        {
            base.Initialize(strategy);


        }
    }
}
