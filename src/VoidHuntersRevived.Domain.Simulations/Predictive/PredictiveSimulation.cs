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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    internal sealed class PredictiveSimulation : Simulation
    {
        private readonly ILockstepSimulation _lockstep;
        private Step _step;
        private double _lastStepTime;
        private IPredictiveSynchronizationEngine[] _synchronizations;
        private readonly PredictionService _predictions;

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
        }

        public override void Initialize(ISimulationService simulations)
        {
            base.Initialize(simulations);

            _lockstep.OnEvent += this.HandleLockstepEvent;
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
        }

        public override void Enqueue(EventDto data)
        {
            _predictions.Predict(data);
        }

        public override void Publish(EventDto data)
        {
            _predictions.Predict(data);
        }

        private void HandleLockstepEvent(EventDto args)
        {
            _predictions.Verify(args);
        }
    }
}
