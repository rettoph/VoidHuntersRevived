using Guppy.Core.Messaging.Common;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Attributes;
using Guppy.Core.Network.Common.Enums;
using Microsoft.Xna.Framework;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerFilter(PeerType.Server)]
    public sealed class LockstepStrategy_Server : LockstepStrategy,
        ISubscriber<INetIncomingMessage<EventDto>>
    {
        private readonly IBus _bus;
        private readonly List<EventDto> _inputs;

        public LockstepStrategy_Server(
            IBus bus,
            Lazy<ISimulation> simulation,
            Lazy<IEngineService> engines,
            Lazy<ILogger> logger) : base(simulation, engines, logger)
        {
            Fix64 stepInterval = Settings.StepInterval.Value;

            _bus = bus;
            _inputs = new List<EventDto>();
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);

            _bus.Subscribe(this);

            this.Input(VhId.NewId(), new Simulation_Begin());
        }

        public override void Dispose()
        {
            base.Dispose();

            _bus.Unsubscribe(this);
        }

        protected override void DoStep(Step step)
        {
            base.DoStep(step);

            this.timeSinceStep -= this.stepTimeSpan;
        }

        protected override bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step)
        {
            if (this.stepsSinceTick > this.stepsPerTick)
            {
                throw new Exception();
            }

            if (this.stepsSinceTick == this.stepsPerTick)
            {
                step = null;
                return false;
            }

            if (this.timeSinceStep < this.stepTimeSpan)
            {
                step = null;
                return false;
            }

            this.step.TotalTime += this.step.ElapsedTime;
            step = this.step;
            return true;
        }

        public override void Input(VhId sourceId, IInputData data)
        {
            _inputs.Add(new EventDto()
            {
                SourceId = sourceId,
                Data = data
            });
        }

        protected override bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next)
        {
            if (this.stepsSinceTick != this.stepsPerTick)
            {
                next = null;
                return false;
            }

            next = current.Next(_inputs.ToArray());
            _inputs.Clear();

            return true;
        }

        public void Process(in Guid messsageId, INetIncomingMessage<EventDto> message)
        {
            if (message.Body.Data is not IInputData input)
            {
                throw new InvalidOperationException();
            }

            this.Input(message.Body.SourceId, input);
        }
    }
}
