using Guppy.Core.Common.Collections;
using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Messages;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Domain.Simulations.Predictive.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public class PredictiveEventService(IMessageBus messageBus, ILogger logger) : BaseEventService(messageBus, logger)
    {
        private static readonly Pool<PredictedEvent> _predictionPool = new(ushort.MaxValue);
        private readonly DictionaryQueue<Id<IStepEvent>, PredictedEvent> _predictedEvents = new();
        private Fix64 _cleanedAt = Fix64.Zero;
        private readonly Queue<EnqueuedStepEvent> _confirmedEvents = new();

        public override void Input(EnqueuedStepInput input)
        {
            this.Publish(input.Id, input.Data);
        }

        public override void Publish(Id<IStepEvent> id, IStepEvent @event)
        {
            if (@event.IsPredictable == false && @event.IsPrivate == false)
            { // The event must be both non-predictable and public in order for us to skip it
                // What would it even mean for a private event to be non predictable? 
                // It wouldnt happen on the predictive strategy and never get synced by the lockstep
                this.logger.Verbose("Unable to predict {EventName}, {EventId}; IsPredictable = {IsPredictable}.", @event.GetType().Name, id, @event.IsPredictable);
                return;
            }

            ref PredictedEvent? predictiveEvent = ref this._predictedEvents.GetOrEnqueue(id, out bool exists);
            if (exists == true)
            {
                this.logger.Error("Unable to predict {EventName}, {EventId}; duplicate event?", @event.GetType().Name, id);
                return;
            }

            predictiveEvent = this.GetPredictionEvent(id, @event);
            this.logger.Verbose("Predicting {EventName}, {EventId}", @event.GetType().Name, id);

            if (@event.IsPrivate)
            { // Private events may as well be immidiately confirmed, right? They will never get verified
                predictiveEvent.Status = PredictedEventStatus.Confirmed;
            }

            base.Publish(id, @event);
        }

        public void Confirm(Id<IStepEvent> id, IStepEvent @event)
        {
            this._confirmedEvents.Enqueue(new EnqueuedStepEvent(id, @event));
        }

        public void CleanFailedPredictions(Step step)
        {
            while (this._predictedEvents.TryPeek(out PredictedEvent? prediction) && prediction.IsExpired(step))
            {
                if (prediction.Status == PredictedEventStatus.Unconfirmed)
                {
                    this.Revert(prediction.Id, prediction.Event);

                    prediction.Status = PredictedEventStatus.Reverted;
                }

                if (this._predictedEvents.TryDequeue(out PredictedEvent? oldPrediction))
                {
                    _predictionPool.TryReturn(oldPrediction);
                }
            }

            this._cleanedAt = step.TotalTime;
        }

        public void CleanConfirmedPredictions()
        {
            while (this._confirmedEvents.TryDequeue(out EnqueuedStepEvent? confirmedEvent))
            {
                if (confirmedEvent.Data is EndOfTick endOfTick)
                {
                    this.logger.Verbose("End of Tick {TickId}", endOfTick.TickId);

                    break;
                }

                this.logger.Verbose("Confirming {EventName}, {EventId}", confirmedEvent.Data.GetType().Name, confirmedEvent.Id.Value);
                if (this._predictedEvents.TryGet(confirmedEvent.Id, out PredictedEvent? published) == false)
                {
                    published = this.GetPredictionEvent(confirmedEvent.Id, confirmedEvent.Data);
                    this._predictedEvents.TryEnqueue(confirmedEvent.Id, published);
                    base.Publish(confirmedEvent.Id, confirmedEvent.Data);
                }

                published.Status = PredictedEventStatus.Confirmed;
            }
        }

        private PredictedEvent GetPredictionEvent(Id<IStepEvent> id, IStepEvent @event)
        {
            if (!_predictionPool.TryPull(out PredictedEvent? prediction))
            {
                prediction = new PredictedEvent();
            }

            prediction.Status = @event.IsPrivate ? PredictedEventStatus.Confirmed : PredictedEventStatus.Unconfirmed;
            prediction.SetEvent(id, @event, this._cleanedAt);
            return prediction;
        }

        private void Revert(Id<IStepEvent> id, IStepEvent data)
        {
            data.Revert(id.Value, this.messageBus);
        }
    }
}
