using Guppy.Core.Common;
using Guppy.Core.Logging.Common;
using VoidHuntersRevived.Common;
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
        ILogger logger
    ) : Strategy(StrategyTypeEnum.Predictive, scope, eventService, logger),
        IPredictiveStrategy
    {
        private readonly PredictiveStepEventService _eventService = eventService;
        private ILockstepStrategy _lockstep = null!;
        private readonly Step _step = new();
        private readonly double _lastStepTime;
        private IPredictiveSynchronizationSystem[] _synchronizations = [];


        protected override void Initialize()
        {
            base.Initialize();

            this._lockstep = this.Simulation.First(StrategyTypeEnum.Lockstep) as ILockstepStrategy ?? throw new NotImplementedException();
            this._lockstep.Events.OnEvent += this.HandleLockstepEvent;
            this._synchronizations = this.Systems.GetAll<IPredictiveSynchronizationSystem>().ToArray();

            foreach (IPredictiveSynchronizationSystem synchronization in this._synchronizations)
            {
                synchronization.Initialize(this._lockstep);
            }
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