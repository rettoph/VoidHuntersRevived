using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Game.Common.Systems;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    public class EngineSystem(IStrategy strategy, EnginesRoot enginesRoot) : ISceneSystem, IInitializeSystem
    {
        private readonly IStrategy _strategy = strategy;
        private readonly EnginesRoot _enginesRoot = enginesRoot;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Setup)]
        public void Initialize()
        {
            foreach (IEngine engine in this._strategy.Systems.GetAll<IEngine>())
            {
                this._enginesRoot.AddEngine(engine);
            }
        }
    }
}
