using Guppy.Common;
using Guppy.Common.Providers;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using Autofac;
using VoidHuntersRevived.Common.Simulations.Events;
using Guppy.Messaging;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerFilter(PeerType.Server)]
    internal sealed class LockstepSimulation_Server : LockstepSimulation,
        ISubscriber<INetIncomingMessage<EventDto>>
    {
        private readonly IBus _bus;
        private readonly List<EventDto> _inputs;
        private TimeSpan _timeSinceStep;
        private TimeSpan _stepTimeSpan;
        private Step _step;

        internal readonly int _stepsPerTick;
        internal int _stepsSinceTick;

        public LockstepSimulation_Server(
            ISettingProvider settings,
            ILifetimeScope scope,
            IBus bus) : base(scope)
        {
            Fix64 stepInterval = settings.Get(Settings.StepInterval).Value;

            _bus = bus;
            _stepsSinceTick = 0;
            _inputs = new List<EventDto>();
            _stepsPerTick = settings.Get(Settings.StepsPerTick).Value;
            _stepTimeSpan = TimeSpan.FromSeconds((float)stepInterval);
            _step = new Step()
            {
                ElapsedTime = stepInterval,
                TotalTime = stepInterval
            };
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

        public override void Update(GameTime realTime)
        {
            _timeSinceStep += realTime.ElapsedGameTime;

            base.Update(realTime);
        }

        protected override bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step)
        {
            if (_stepsSinceTick >= _stepsPerTick)
            {
                step = null;
                return false;
            }

            if(_timeSinceStep < _stepTimeSpan)
            {
                step = null;
                return false;
            }

            _step.TotalTime += _step.ElapsedTime;
            step = _step;
            return true;
        }

        protected override void DoStep(Step step)
        {
            base.DoStep(step);

            _stepsSinceTick++;
            _timeSinceStep -= _stepTimeSpan;
        }

        public override void Input(VhId sender, IInputData data)
        {
            _inputs.Add(new EventDto()
            {
                Sender = sender,
                Data = data
            });
        }

        protected override bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next)
        {
            if(_stepsSinceTick != _stepsPerTick)
            {
                next = null;
                return false;
            }

            next = current.Next(_inputs.ToArray());
            _inputs.Clear();

            return true;
        }

        protected override void DoTick(Tick tick)
        {
            base.DoTick(tick);

            _stepsSinceTick = 0;
        }

        public void Process(in Guid messsageId, INetIncomingMessage<EventDto> message)
        {
            if (message.Body.Data is not IInputData)
            {
                throw new InvalidOperationException();
            }

            this.Input(message.Body.Sender, (IInputData)message.Body.Data);
        }
    }
}
