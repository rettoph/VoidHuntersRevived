using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Common.Providers;
using Guppy.Core.Network.Common;
using Guppy.Core.Resources.Common.Services;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    public sealed class LockstepStrategy_Client(
        INetScope<IStrategy> netScope,
        TickBuffer ticks,
        ISettingService settings,
        Lazy<IEngineService> engineService,
        Lazy<ILoggerService> loggerService) : LockstepStrategy(settings, engineService, loggerService),
        IClientEngine,
        IDisposable
    {
        private readonly INetScope<IStrategy> _netScope = netScope;

        public TickBuffer TickBuffer { get; } = ticks;

        public override void Update(GameTime realTime)
        {
            base.Update(realTime);
        }

        protected override void DoStep(Step step)
        {
            base.DoStep(step);

            this.TimeSinceStep = TimeSpan.Zero;
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

            if (this.TimeSinceStep < this.StepTimeSpan && this.TickBuffer.Count == 0)
            {
                return false;
            }

            return true;
        }

        protected override bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next)
        {
            if (this.StepsSinceTick > this.StepsPerTick)
            {
                throw new Exception();
            }

            if (this.StepsSinceTick < this.StepsPerTick)
            {
                next = null;
                return false;
            }

            return this.TickBuffer.TryPop(current.Id + 1, out next);
        }

        public override void Input(VhId sourceId, IInputData data)
        {
            this._netScope.CreateMessage(new EventDto()
            {
                SourceId = sourceId,
                Data = data
            });
        }
    }
}