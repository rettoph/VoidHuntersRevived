using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common.Services;
using Guppy.Core.Messaging.Common;
using Guppy.Core.Messaging.Common.Enums;
using Guppy.Core.Network.Common;
using Guppy.Core.Resources.Common.Services;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    public sealed class LockstepStrategy_Server(
        ISettingService settings,
        IGuppyScope scope,
        LockstepStepEventService eventService,
        Lazy<ILoggerService> loggerService
    ) : LockstepStrategy(settings, scope, eventService, loggerService),
        INetIncomingMessageSubscriber<EnqueuedStepInput>,
        ISubscriber<SubscriberSequenceGroupEnum, EnqueuedStepInput>
    {
        private readonly List<EnqueuedStepInput> _inputs = [];

        protected override void Initialize()
        {
            base.Initialize();

            this.Events.Input(NameSpace<LockstepStrategy_Server>.Instance, new Simulation_Begin());
        }

        protected override void DoStep(Step step)
        {
            base.DoStep(step);

            this.TimeSinceStep -= this.StepTimeSpan;
        }

        protected override bool ShouldStep(GameTime realTime)
        {
            if (this.StepsSinceTick > this.StepsPerTick)
            {
                throw new Exception();
            }

            if (this.StepsSinceTick == this.StepsPerTick)
            {
                return false;
            }

            if (this.TimeSinceStep < this.StepTimeSpan)
            {
                return false;
            }

            return true;
        }

        protected override bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next)
        {
            if (this.StepsSinceTick != this.StepsPerTick)
            {
                next = null;
                return false;
            }

            next = current.Next([.. this._inputs]);
            this._inputs.Clear();

            return true;
        }

        [SequenceGroup<SubscriberSequenceGroupEnum>(SubscriberSequenceGroupEnum.Process)]
        public void Process(INetIncomingMessage<EnqueuedStepInput> message)
        {
            this._inputs.Add(message.Body);
        }

        [SequenceGroup<SubscriberSequenceGroupEnum>(SubscriberSequenceGroupEnum.Process)]
        public void Process(EnqueuedStepInput message)
        {
            this._inputs.Add(message);
        }
    }
}