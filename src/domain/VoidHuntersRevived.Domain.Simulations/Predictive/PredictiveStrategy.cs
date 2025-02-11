using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Common;
using Guppy.Core.Common.Collections;
using Guppy.Core.Logging.Common.Services;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Predictive;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Messages;
using VoidHuntersRevived.Domain.Simulations.Predictive.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    public sealed class PredictiveStrategy(
        IGuppyScope scope,
        Lazy<ILoggerService> loggerService) : Strategy(StrategyTypeEnum.Predictive, scope, loggerService), IPredictiveStrategy
    {
        private static readonly Pool<PredictedEvent> _predictionPool = new(ushort.MaxValue);
        private ILockstepStrategy _lockstep = null!;
        private readonly Step _step = new();
        private double _lastStepTime;
        private IPredictiveSynchronizationSystem[] _synchronizations = [];
        private readonly DictionaryQueue<Id<IStepEvent>, PredictedEvent> _predictedEvents = new();
        private readonly Queue<EnqueuedStepEvent> _confirmedEvents = new();

        protected override void Initialize()
        {
            base.Initialize();

            this._lockstep = this.Simulation.First(StrategyTypeEnum.Lockstep) as ILockstepStrategy ?? throw new NotImplementedException();
            this._lockstep.OnEvent += this.HandleLockstepEvent;
            this._synchronizations = this.Systems.OfType<IPredictiveSynchronizationSystem>().ToArray();

            foreach (IPredictiveSynchronizationSystem synchronization in this._synchronizations)
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

            foreach (IPredictiveSynchronizationSystem synchronization in this._synchronizations)
            {
                synchronization.Synchronize(step);
            }

            while (this._predictedEvents.TryPeek(out PredictedEvent? prediction) && prediction.IsExpired(this.CurrentStep))
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
        }

        public override void Input(EnqueuedStepInput input)
        {
            this.Publish(input.Id, input.Data);
        }

        public override void Publish(Id<IStepEvent> id, IStepEvent data)
        {
            if (data.IsPredictable == false && data.IsPrivate == false)
            { // The event must be both non-predictable and public in order for us to skip it
                // What would it even mean for a private event to be non predictable? 
                // It wouldnt happen on the predictive strategy and never get synced by the lockstep
                this.logger.Verbose("Unable to predict {EventName}, {EventId}; IsPredictable = {IsPredictable}.", data.GetType().Name, id, data.IsPredictable);
                return;
            }

            ref PredictedEvent? predictiveEvent = ref this._predictedEvents.GetOrEnqueue(id, out bool exists);
            if (exists == true)
            {
                this.logger.Error("Unable to predict {EventName}, {EventId}; duplicate event?", data.GetType().Name, id);
                return;
            }

            predictiveEvent = this.GetPredictionEvent(id, data);
            this.logger.Verbose("Predicting {EventName}, {EventId}", data.GetType().Name, id);

            if (data.IsPrivate)
            { // Private events may as well be immidiately confirmed, right? They will never get verified
                predictiveEvent.Status = PredictedEventStatus.Confirmed;
            }

            base.Publish(id, data);
        }

        private void Confirm()
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

        private void HandleLockstepEvent(Id<IStepEvent> id, IStepEvent @event)
        {
            if (@event.IsPrivate == false)
            {
                this._confirmedEvents.Enqueue(new EnqueuedStepEvent(id, @event));
            }
        }

        private PredictedEvent GetPredictionEvent(Id<IStepEvent> id, IStepEvent @event)
        {
            if (!_predictionPool.TryPull(out PredictedEvent? prediction))
            {
                prediction = new PredictedEvent();
            }

            prediction.Status = @event.IsPrivate ? PredictedEventStatus.Confirmed : PredictedEventStatus.Unconfirmed;
            prediction.SetEvent(id, @event, this.CurrentStep);
            return prediction;
        }
    }
}