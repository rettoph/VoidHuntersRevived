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
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Predictive.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    internal sealed class PredictiveSimulation : Simulation
    {
        private readonly ILockstepSimulation _lockstep;
        private Step _step;
        private double _lastStepTime;
        private IPredictiveSynchronizationEngine[] _synchronizations;
        private readonly PredictionService _predictions;
        private readonly Queue<Tick> _verifiableTicks;

        public PredictiveSimulation(
            ILogger logger,
            IFiltered<ILockstepSimulation> lockstep,
            ISpaceFactory spaceFactory, 
            IFilteredProvider filtered, 
            IBus bus) : base(SimulationType.Predictive, spaceFactory, filtered, bus)
        {
            _step = new Step();
            _lockstep = lockstep.Instance;
            _synchronizations = Array.Empty<IPredictiveSynchronizationEngine>();
            _predictions = new PredictionService(logger, this.publisher);
            _verifiableTicks = new Queue<Tick>();
        }

        public override void Initialize(ISimulationService simulations)
        {
            base.Initialize(simulations);

            _lockstep.OnTick += this.HandleLockstepTick;
            _synchronizations = this.World.Engines.OfType<IPredictiveSynchronizationEngine>().ToArray();
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
                synchronization.Synchronize(_lockstep, step);
            }

            _predictions.Prune();

            if(_verifiableTicks.TryDequeue(out Tick? verifiableTick))
            {
                foreach(EventDto verifiableEvent in verifiableTick.Events)
                {
                    _predictions.Verify(verifiableEvent);
                }
            }
        }

        protected override void Publish(EventDto data)
        {
            _predictions.Predict(data);
        }

        private void HandleLockstepTick(Tick tick)
        {
            if(tick.Events.Length == 0)
            {
                return;
            }

            _verifiableTicks.Enqueue(tick);
        }
    }
}
