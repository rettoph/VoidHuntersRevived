using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Core.Components.Scene
{
    public class SimulationFrameComponent(ISimulationService simulationService) : ISceneComponent<VoidHuntersGameScene>, IDrawableComponent, IUpdatableComponent
    {
        private readonly ISimulationService _simulationService = simulationService;

        [SequenceGroup<InitializeComponentSequenceGroupEnum>(InitializeComponentSequenceGroupEnum.Initialize)]
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