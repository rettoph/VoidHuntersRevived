using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Attributes;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.Resources.Common.Services;
using Microsoft.Xna.Framework;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerFilter(PeerType.Client)]
    public sealed class LockstepStrategy_Client(
        INetScope<IStrategy> netScope,
        TickBuffer ticks,
        ISettingService settings,
        Lazy<IEngineService> engineService,
        Lazy<ILogger> logger) : LockstepStrategy(settings, engineService, logger),
        IDisposable
    {
        private readonly INetScope<IStrategy> _netScope = netScope;

        private readonly TickBuffer _tickBuffer = ticks;

        public TickBuffer TickBuffer => _tickBuffer;

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

            if (this.TimeSinceStep < this.StepTimeSpan && _tickBuffer.Count == 0)
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

            return _tickBuffer.TryPop(current.Id + 1, out next);
        }

        public override void Input(VhId sourceId, IInputData data)
        {
            _netScope.CreateMessage(new EventDto()
            {
                SourceId = sourceId,
                Data = data
            });
        }
    }
}
