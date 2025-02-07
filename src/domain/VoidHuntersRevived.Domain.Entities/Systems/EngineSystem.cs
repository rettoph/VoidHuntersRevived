using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    public class EngineSystem(EnginesRoot enginesRoot) : ISceneSystem, IOnInitializeSystem<IStrategy>
    {
        private readonly EnginesRoot _enginesRoot = enginesRoot;

        [SequenceGroup<OnInitializeSequenceGroupEnum>(OnInitializeSequenceGroupEnum.Begin)]
        public void OnInitialize(IStrategy strategy)
        {
            foreach (IEngine engine in strategy.Systems.OfType<IEngine>())
            {
                this._enginesRoot.AddEngine(engine);
            }
        }
    }
}
