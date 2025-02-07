using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.Common.Systems;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Core.Components.Scene
{
    public class SimulationFrameSystem(ISimulationService simulationService) : ISceneSystem, IDrawSystem, IUpdateSystem
    {
        private readonly ISimulationService _simulationService = simulationService;

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