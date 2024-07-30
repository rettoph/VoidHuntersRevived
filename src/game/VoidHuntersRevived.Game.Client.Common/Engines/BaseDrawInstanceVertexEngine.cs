using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Game.Client.Common.Services;

namespace VoidHuntersRevived.Game.Client.Common.Engines
{
    public abstract class BaseDrawInstanceVertexEngine<TInstancedPrimitiveVertexService> : StrategyEngine, IStepEngine<GameTime>
        where TInstancedPrimitiveVertexService : IInstanceVertexService
    {
        private IStepGroupEngine<TInstancedPrimitiveVertexService> _stepEngines;

        protected readonly TInstancedPrimitiveVertexService instancedPrimitiveVertexService;

        public string name => nameof(BaseDrawInstanceVertexEngine<TInstancedPrimitiveVertexService>);

        protected BaseDrawInstanceVertexEngine(TInstancedPrimitiveVertexService instancedPrimitiveVertexService)
        {
            _stepEngines = null!;

            this.instancedPrimitiveVertexService = instancedPrimitiveVertexService;
        }

        public override void Initialize(IStrategy strategy)
        {
            base.Initialize(strategy);

            _stepEngines = strategy.Engines.All().CreateSequencedStepEnginesGroup<TInstancedPrimitiveVertexService, DrawSequence>(DrawSequence.Draw);

            this.instancedPrimitiveVertexService.Initialize();
        }

        public void Step(in GameTime param)
        {
            _stepEngines.Step(this.instancedPrimitiveVertexService);

            this.Draw(param);
        }

        protected abstract void Draw(GameTime gameTime);
    }
}
