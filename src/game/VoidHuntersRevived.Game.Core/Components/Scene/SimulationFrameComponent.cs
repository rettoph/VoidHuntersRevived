using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Core.Components.Scene
{
    [AutoLoad]
    [SceneFilter<VoidHuntersGameScene>]
    [SequenceGroup<DrawSequence>(DrawSequence.Draw)]
    internal class SimulationFrameComponent : ISceneComponent, IDrawableComponent, IUpdatableComponent
    {
        private readonly ISimulationService _simulationService;

        public SimulationFrameComponent(ISimulationService simulationService)
        {
            _simulationService = simulationService;
        }

        [SequenceGroup<DrawComponentSequenceGroup>(DrawComponentSequenceGroup.PostDraw)]
        public void Draw(GameTime gameTime)
        {
            _simulationService.Draw(gameTime);
        }

        [SequenceGroup<InitializeComponentSequenceGroup>(InitializeComponentSequenceGroup.Initialize)]
        public void Initialize(IScene scene)
        {
            //
        }

        [SequenceGroup<UpdateComponentSequenceGroup>(UpdateComponentSequenceGroup.Update)]
        public void Update(GameTime gameTime)
        {
            _simulationService.Update(gameTime);
        }
    }
}
