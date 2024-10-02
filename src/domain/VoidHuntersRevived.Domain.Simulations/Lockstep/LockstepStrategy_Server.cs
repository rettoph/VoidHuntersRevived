using Guppy.Core.Messaging.Common;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Attributes;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.Resources.Common.Services;
using Microsoft.Xna.Framework;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerFilter(PeerType.Server)]
    public sealed class LockstepStrategy_Server(
        IBus bus,
        ISettingService settings,
        Lazy<ISimulation> simulation,
        Lazy<IEngineService> engineService,
        Lazy<ILogger> logger) : LockstepStrategy(settings, simulation, engineService, logger),
        ISubscriber<INetIncomingMessage<EventDto>>
    {
        private readonly IBus _bus = bus;
        private readonly List<EventDto> _inputs = [];

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
            if (this.StepsSinceTick != this.StepsPerTick)
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
