using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Common.Collections;
using Guppy.Core.Common.Providers;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Graphics.Common.Constants;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Predictive;
using VoidHuntersRevived.Domain.Simulations.Messages;
using VoidHuntersRevived.Domain.Simulations.Predictive.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    [SetSceneConfiguration<bool>(GraphicsSceneConfigurationKeys.SceneHasGraphicsEnabled, true)]
    public sealed class PredictiveStrategy(
        Lazy<IEngineService> engineService,
        Lazy<ILoggerService> loggerService) : Strategy(StrategyTypeEnum.Predictive, engineService, loggerService), IPredictiveStrategy
    {
        private static readonly Pool<PredictedEvent> _predictionPool = new(ushort.MaxValue);
        private ILockstepStrategy _lockstep = null!;
        private readonly Step _step = new();
        private double _lastStepTime;
        private IPredictiveSynchronizationEngine[] _synchronizations = [];
        private readonly DictionaryQueue<VhId, PredictedEvent> _predictedEvents = new();
        private readonly Queue<EventDto> _confirmedEvents = new();

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);

            this._lockstep = simulation.First(StrategyTypeEnum.Lockstep) as ILockstepStrategy ?? throw new NotImplementedException();
            this._lockstep.OnEvent += this.HandleLockstepEvent;
            this._synchronizations = this.Engines.OfType<IPredictiveSynchronizationEngine>().ToArray();

            foreach (IPredictiveSynchronizationEngine synchronization in this._synchronizations)
            {
                synchronization.Initialize(this._lockstep);
            }
        }

        protected override bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step)
        {
            if (this._lastStepTime == realTime.TotalGameTime.TotalSeconds)
            {
                step = default!;
                return false;
            }

            this._step.ElapsedTime = (Fix64)realTime.ElapsedGameTime.TotalSeconds;
            this._step.TotalTime += this._step.ElapsedTime;
            this._lastStepTime = realTime.TotalGameTime.TotalSeconds;

            step = this._step;
            return true;
        }

        protected override void DoStep(Step step)
        {
            this.Confirm();

            base.DoStep(step);

            foreach (IPredictiveSynchronizationEngine synchronization in this._synchronizations)
            {
                synchronization.Synchronize(step);
            }

            while (this._predictedEvents.TryPeek(out PredictedEvent? prediction) && prediction.IsExpired(this.CurrentStep))
            {
                if (prediction.Status == PredictedEventStatus.Unconfirmed)
                {
                    this.Revert(prediction.Event);

                    prediction.Status = PredictedEventStatus.Reverted;
                }

                if (this._predictedEvents.TryDequeue(out PredictedEvent? oldPrediction))
                {
                    _predictionPool.TryReturn(ref oldPrediction);
                }
            }
        }

        public override void Input(VhId sourceId, IInputData data) => this.Publish(new EventDto()
        {
            SourceId = sourceId,
            Data = data
        });

        public override void Publish(EventDto @event)
        {
            if (@event.Data.IsPredictable == false && @event.Data.IsPrivate == false)
            { // The event must be both non-predictable and public in order for us to skip it
                // What would it even mean for a private event to be non predictable? 
                // It wouldnt happen on the predictive strategy and never get synced by the lockstep
                this.logger.Verbose("Unable to predict {EventName}, {EventId}; IsPredictable = {IsPredictable}.", @event.Data.GetType().Name, @event.Id.Value, @event.Data.IsPredictable);
                return;
            }

            ref PredictedEvent? predictiveEvent = ref this._predictedEvents.GetOrEnqueue(@event.Id, out bool exists);
            if (exists == true)
            {
                this.logger.Error("Unable to predict {EventName}, {EventId}; duplicate event?", @event.Data.GetType().Name, @event.Id.Value);
                return;
            }

            predictiveEvent = this.GetPredictionEvent(@event);
            this.logger.Verbose("Predicting {EventName}, {EventId}", @event.Data.GetType().Name, @event.Id.Value);

            if (@event.Data.IsPrivate)
            { // Private events may as well be immidiately confirmed, right? They will never get verified
                predictiveEvent.Status = PredictedEventStatus.Confirmed;
            }

            base.Publish(@event);
        }

        private void Confirm()
        {
            while (this._confirmedEvents.TryDequeue(out EventDto? confirmedEvent))
            {
                if (confirmedEvent.Data is EndOfTick endOfTick)
                {
                    this.logger.Verbose("End of Tick {TickId}", endOfTick.TickId);

                    break;
                }

                this.logger.Verbose("Confirming {EventName}, {EventId}", confirmedEvent.Data.GetType().Name, confirmedEvent.Id.Value);

                if (this._predictedEvents.TryGet(confirmedEvent.Id, out PredictedEvent? published) == false)
                {
                    published = this.GetPredictionEvent(confirmedEvent);
                    this._predictedEvents.TryEnqueue(confirmedEvent.Id, published);
                    base.Publish(confirmedEvent);
                }

                published.Status = PredictedEventStatus.Confirmed;
            }
        }

        private void HandleLockstepEvent(EventDto @event)
        {
            if (@event.Data.IsPrivate == false)
            {
                this._confirmedEvents.Enqueue(@event);
            }
        }

        private PredictedEvent GetPredictionEvent(EventDto @event)
        {
            if (!_predictionPool.TryPull(out PredictedEvent? prediction))
            {
                prediction = new PredictedEvent();
            }

            prediction.Status = @event.Data.IsPrivate ? PredictedEventStatus.Confirmed : PredictedEventStatus.Unconfirmed;
            prediction.SetEvent(@event, this.CurrentStep);
            return prediction;
        }
    }
}