using Guppy.Common;
using Guppy.Common.Providers;
using Microsoft.Xna.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using Autofac;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    internal sealed class PredictiveSimulation : Simulation
    {
        private readonly ILockstepSimulation _lockstep;
        private Step _step;
        private double _lastStepTime;
        private IPredictiveSynchronizationEngine[] _synchronizations;
        private readonly Queue<EventDto> _confirmedEvents;

        public PredictiveSimulation(
            IFiltered<ILockstepSimulation> lockstep,
            ILifetimeScope scope) : base(SimulationType.Predictive, scope)
        {
            _step = new Step();
            _lockstep = lockstep.Instance;
            _synchronizations = Array.Empty<IPredictiveSynchronizationEngine>();
            _confirmedEvents = new Queue<EventDto>();
        }

        public override void Initialize(ISimulationService simulations)
        {
            base.Initialize(simulations);

            _lockstep.OnEvent += this.HandleLockstepTick;
            _synchronizations = this.Engines.OfType<IPredictiveSynchronizationEngine>().ToArray();

            foreach(IPredictiveSynchronizationEngine synchronization in _synchronizations)
            {
                synchronization.Initialize(_lockstep);
            }
        }

        protected override bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step)
        {
            if (_lastStepTime == realTime.TotalGameTime.TotalSeconds)
            {
                step = default!;
                return false;
            }

            _step.ElapsedTime = (Fix64)realTime.ElapsedGameTime.TotalSeconds;
            _step.TotalTime += _step.ElapsedTime;
            _lastStepTime = realTime.TotalGameTime.TotalSeconds;

            step = _step;
            return true;
        }

        protected override void DoStep(Step step)
        {
            base.DoStep(step);

            foreach(IPredictiveSynchronizationEngine synchronization in _synchronizations)
            {
                synchronization.Synchronize(step);
            }

            this.Events.Revert();

            while(_confirmedEvents.TryDequeue(out EventDto? confirmedEvent))
            {
                this.Events.Confirm(confirmedEvent);
            }
        }

        private void HandleLockstepTick(EventDto @event)
        {
            _confirmedEvents.Enqueue(@event);
        }

        public override void Input(VhId sender, IInputData data)
        {
            this.Publish(sender, data);
        }
    }
}
