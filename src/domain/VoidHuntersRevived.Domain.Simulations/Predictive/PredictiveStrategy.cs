using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Common;
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
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    public sealed class PredictiveStrategy(
        IGuppyScope scope,
        PredictiveStepEventService eventService,
        Lazy<ILoggerService> loggerService
    ) : Strategy(StrategyTypeEnum.Predictive, scope, eventService, loggerService),
        IPredictiveStrategy
    {
        private readonly PredictiveStepEventService _eventService = eventService;
        private ILockstepStrategy _lockstep = null!;
        private readonly Step _step = new();
        private double _lastStepTime;
        private IPredictiveSynchronizationSystem[] _synchronizations = [];


        protected override void Initialize()
        {
            base.Initialize();

            this._lockstep = this.Simulation.First(StrategyTypeEnum.Lockstep) as ILockstepStrategy ?? throw new NotImplementedException();
            this._lockstep.Events.OnEvent += this.HandleLockstepEvent;
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
            this._eventService.CleanConfirmedPredictions();

            base.DoStep(step);

            foreach (IPredictiveSynchronizationSystem synchronization in this._synchronizations)
            {
                synchronization.Synchronize(step);
            }

            this._eventService.CleanFailedPredictions(step);
        }

        private void HandleLockstepEvent(Id<IStepEvent> id, IStepEvent @event)
        {
            if (@event.IsPrivate == false)
            {
                this._eventService.Confirm(id, @event);
            }
        }
    }
}