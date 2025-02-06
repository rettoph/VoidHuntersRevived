using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Game.Common.Systems;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Core.Components.Scene
{
    public class SimulationFrameSystem(ISimulationService simulationService) : ISceneSystem<VoidHuntersGameScene>, IDrawSystem, IUpdateSystem
    {
        private readonly ISimulationService _simulationService = simulationService;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Initialize)]
        public void Initialize(VoidHuntersGameScene scene)
        {
        }

        [SequenceGroup<DrawSequenceGroupEnum>(DrawSequenceGroupEnum.PostDraw)]
        public void Draw(GameTime gameTime)
        {
            this._simulationService.Draw(gameTime);
        }

        [SequenceGroup<UpdateSequenceGroupEnum>(UpdateSequenceGroupEnum.Update)]
        public void Update(GameTime gameTime)
        {
            this._simulationService.Update(gameTime);
        }
    }
}