using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.Common.Systems;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    public class StepServiceUpdateSystem(IStepService stepService) : ISceneSystem, IUpdateSystem
    {
        private readonly IStepService _stepService = stepService;

        [SequenceGroup<UpdateSequenceGroupEnum>(UpdateSequenceGroupEnum.Update)]
        public void Update(GameTime gameTime)
        {
            this._stepService.Update(gameTime);
        }
    }
}
