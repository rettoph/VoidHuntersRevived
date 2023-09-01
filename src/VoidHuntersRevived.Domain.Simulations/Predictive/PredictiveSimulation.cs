﻿using Guppy.Common;
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
using System.Reflection;
using VoidHuntersRevived.Common.Simulations.Attributes;
using Guppy.Common.Collections;
using VoidHuntersRevived.Domain.Simulations.Predictive.Enums;
using VoidHuntersRevived.Domain.Simulations.Utilities;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    internal sealed class PredictiveSimulation : Simulation
    {
        private readonly ILockstepSimulation _lockstep;
        private Step _step;
        private double _lastStepTime;
        private IPredictiveSynchronizationEngine[] _synchronizations;
        private readonly DictionaryQueue<VhId, PredictedEvent> _predictedEvents;
        private readonly Queue<EventDto> _confirmedEvents;

        public PredictiveSimulation(
            IFiltered<ILockstepSimulation> lockstep,
            ILifetimeScope scope) : base(SimulationType.Predictive, scope)
        {
            _step = new Step();
            _lockstep = lockstep.Instance;
            _synchronizations = Array.Empty<IPredictiveSynchronizationEngine>();
            _predictedEvents = new DictionaryQueue<VhId, PredictedEvent>();
            _confirmedEvents = new Queue<EventDto>();
        }

        public override void Initialize(ISimulationService simulations)
        {
            base.Initialize(simulations);

            _lockstep.OnEvent += this.HandleLockstepEvent;
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

            while (_predictedEvents.TryPeek(out PredictedEvent? prediction) && prediction.Expired)
            {
                if (prediction.Status == PredictedEventStatus.Unconfirmed)
                {
                    this.Revert(prediction.Event);

                    prediction.Status = PredictedEventStatus.Reverted;
                }

                _predictedEvents.TryDequeue(out _);
            }

            this.Confirm();
        }

        public override void Input(VhId sender, IInputData data)
        {
            this.Publish(sender, data);
        }

        protected override void Publish(EventDto @event)
        {
            if(!@event.Data.IsPredictable)
            {
                this.logger.Verbose("{ClassName}::{MethodName} - Unable to predict {EventName}, {EventId}; IsPredictable = {IsPredictable}.", nameof(PredictiveSimulation), nameof(Publish), @event.Data.GetType().Name, @event.Id.Value, @event.Data.IsPredictable);
                return;
            }

            if (!_predictedEvents.TryEnqueue(@event.Id, new PredictedEvent(@event)))
            {
                this.logger.Warning("{ClassName}::{MethodName} - Unable to predict {EventName}, {EventId}; duplicate event?", nameof(PredictiveSimulation), nameof(Publish), @event.Data.GetType().Name, @event.Id.Value);
                return;
            }

            this.logger.Verbose("{ClassName}::{MethodName} - Predicting event {EventName}, {EventId}", nameof(PredictiveSimulation), nameof(Publish), @event.Data.GetType().Name, @event.Id.Value);

            base.Publish(@event);
        }

        private void Confirm()
        {
            while (_confirmedEvents.TryDequeue(out EventDto? confirmedEvent))
            {
                this.logger.Verbose("{ClassName}::{MethodName} - Confirming Event {EventName}, {EventId}", nameof(PredictiveSimulation), nameof(Confirm), confirmedEvent.Data.GetType().Name, confirmedEvent.Id.Value);

                if (!_predictedEvents.TryGet(confirmedEvent.Id, out PredictedEvent? published))
                {
                    published = new PredictedEvent(confirmedEvent);
                    _predictedEvents.TryEnqueue(confirmedEvent.Id, published);
                    base.Publish(confirmedEvent);
                }

                published.Status = PredictedEventStatus.Confirmed;
            }
        }

        private void HandleLockstepEvent(EventDto @event)
        {
            _confirmedEvents.Enqueue(@event);
        }
    }
}
