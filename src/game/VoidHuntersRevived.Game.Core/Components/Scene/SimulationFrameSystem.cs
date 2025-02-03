using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Game.Common.Systems;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Core.Components.Scene
{
    public class SimulationFrameSystem(ISimulationService simulationService) : ISceneSystem<VoidHuntersGameScene>, IDrawableSystem, IUpdatableSystem
    {
        private readonly ISimulationService _simulationService = simulationService;

        [SequenceGroup<InitializeSystemSequenceGroupEnum>(InitializeSystemSequenceGroupEnum.Initialize)]
        public void Initialize(VoidHuntersGameScene scene)
        {
        }

        [SequenceGroup<DrawComponentSequenceGroupEnum>(DrawComponentSequenceGroupEnum.PostDraw)]
        public void Draw(GameTime gameTime)
        {
            this._simulationService.Draw(gameTime);
        }

        [SequenceGroup<UpdateComponentSequenceGroupEnum>(UpdateComponentSequenceGroupEnum.Update)]
        public void Update(GameTime gameTime)
        {
            this._simulationService.Update(gameTime);
        }
    }
}