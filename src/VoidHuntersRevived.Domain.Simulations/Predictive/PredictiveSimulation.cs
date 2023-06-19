using Guppy.Common;
using Guppy.Common.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    internal sealed class PredictiveSimulation : Simulation
    {
        private readonly List<EventDto> _events;
        private Step _step;

        public PredictiveSimulation(
            ISpaceFactory spaceFactory, 
            IFilteredProvider filtered, 
            IBus bus) : base(SimulationType.Predictive, spaceFactory, filtered, bus)
        {
            _events = new List<EventDto>();
            _step = new Step();
        }

        protected override bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step)
        {
            _step.ElapsedTime = (Fix64)realTime.ElapsedGameTime.TotalSeconds;
            _step.TotalTime += _step.ElapsedTime;

            step = _step;
            return true;
        }

        public override void Enqueue(EventDto data)
        {
            _events.Add(data);
        }

        public override void Publish(EventDto data)
        {
            this.publisher.Publish(data);
        }
    }
}
