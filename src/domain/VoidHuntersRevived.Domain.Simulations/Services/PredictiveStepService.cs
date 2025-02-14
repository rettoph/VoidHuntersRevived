using Guppy.Core.Messaging.Common;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public class PredictiveStepService(IMessageBus messageBus) : IStepService
    {
        private readonly IMessageBus _messageBus = messageBus;
        private readonly Step _step = new();

        public void Update(GameTime gameTime)
        {
            this._step.ElapsedTime = (Fix64)gameTime.ElapsedGameTime.TotalSeconds;
            this._step.TotalTime += this._step.ElapsedTime;

            this._messageBus.Publish<StepSequenceGroupEnum, Step>(this._step);
        }
    }
}
