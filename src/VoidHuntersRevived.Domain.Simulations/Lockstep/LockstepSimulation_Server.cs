using Autofac;
using Guppy.Messaging;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerFilter(PeerType.Server)]
    internal sealed class LockstepSimulation_Server : LockstepSimulation,
        ISubscriber<INetIncomingMessage<EventDto>>
    {
        private readonly IBus _bus;
        private readonly List<EventDto> _inputs;

        public LockstepSimulation_Server(
            ISettingProvider settings,
            ILifetimeScope scope,
            IBus bus) : base(settings, scope)
        {
            Fix64 stepInterval = settings.Get(Settings.StepInterval).Value;

            _bus = bus;
            _inputs = new List<EventDto>();
        }

        public override void Initialize(ISimulationService simulations)
        {
            base.Initialize(simulations);

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
            if (message.Body.Data is not IInputData)
            {
                throw new InvalidOperationException();
            }

            this.Input(message.Body.SourceId, (IInputData)message.Body.Data);
        }
    }
}
