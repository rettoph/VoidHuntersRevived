using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using Guppy.Core.Messaging.Common.Enums;
using Guppy.Core.Network.Common;
using Guppy.Core.Resources.Common.Services;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    public sealed class LockstepStrategy_Client(
        INetScope<IStrategy> netScope,
        TickBuffer ticks,
        ISettingService settings,
        IGuppyScope scope,
        LockstepStepEventService eventService,
        ILogger logger
    ) : LockstepStrategy(settings, scope, eventService, logger),
        ISubscriber<SubscriberSequenceGroupEnum, EnqueuedStepInput>
    {
        private readonly INetScope<IStrategy> _netScope = netScope;

        public TickBuffer TickBuffer { get; } = ticks;

        public override void Update(GameTime realTime)
        {
            base.Update(realTime);
        }

        protected override void DoStep(Step step)
        {
            base.DoStep(step);

            this.TimeSinceStep = TimeSpan.Zero;
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

            if (this.TimeSinceStep < this.StepTimeSpan && this.TickBuffer.Count == 0)
            {
                return false;
            }

            return true;
        }

        protected override bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next)
        {
            if (this.StepsSinceTick > this.StepsPerTick)
            {
                throw new Exception();
            }

            if (this.StepsSinceTick < this.StepsPerTick)
            {
                next = null;
                return false;
            }

            return this.TickBuffer.TryPop(current.Id + 1, out next);
        }

        [SequenceGroup<SubscriberSequenceGroupEnum>(SubscriberSequenceGroupEnum.Process)]
        public void Process(EnqueuedStepInput message)
        {
            this._netScope.CreateMessage(message);
        }
    }
}