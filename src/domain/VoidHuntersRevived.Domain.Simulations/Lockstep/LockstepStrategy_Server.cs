using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Common.Providers;
using Guppy.Core.Messaging.Common;
using Guppy.Core.Network.Common;
using Guppy.Core.Resources.Common.Services;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    public sealed class LockstepStrategy_Server(
        IBus bus,
        ISettingService settings,
        Lazy<IEngineService> engineService,
        Lazy<ILoggerService> loggerService) : LockstepStrategy(settings, engineService, loggerService),
        IServerEngine,
        ISubscriber<INetIncomingMessage<EventDto>>
    {
        private readonly List<EventDto> _inputs = [];

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);

            this.Input(VhId.NewId(), new Simulation_Begin());
        }

        protected override void DoStep(Step step)
        {
            base.DoStep(step);

            this.TimeSinceStep -= this.StepTimeSpan;
        }

        protected override bool ShouldStep(GameTime realTime)
        {
            if (this.StepsSinceTick > this.StepsPerTick)
            {
                throw new Exception();
            }

            if (this.StepsSinceTick == this.StepsPerTick)
            {
                return false;
            }

            if (this.TimeSinceStep < this.StepTimeSpan)
            {
                return false;
            }

            return true;
        }

        public override void Input(VhId sourceId, IInputData data)
        {
            this._inputs.Add(new EventDto()
            {
                SourceId = sourceId,
                Data = data
            });
        }

        protected override bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next)
        {
            if (this.StepsSinceTick != this.StepsPerTick)
            {
                next = null;
                return false;
            }

            next = current.Next([.. this._inputs]);
            this._inputs.Clear();

            return true;
        }

        public void Process(in Guid messsageId, INetIncomingMessage<EventDto> message)
        {
            if (message.Body.Data is not IInputData input)
            {
                throw new InvalidOperationException();
            }

            this.Input(message.Body.SourceId, input);
        }
    }
}