using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Attributes;
using Guppy.Core.Network.Common.Enums;
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
    internal sealed class LockstepStrategy_Client : LockstepStrategy,
        IDisposable
    {
        private readonly INetScope<IStrategy> _netScope;

        internal readonly TickBuffer _ticks;

        public LockstepStrategy_Client(
            INetScope<IStrategy> netScope,
            TickBuffer ticks,
            Lazy<ISimulation> simulation,
            Lazy<IEngineService> engines,
            Lazy<ILogger> logger) : base(simulation, engines, logger)
        {
            _netScope = netScope;
            _ticks = ticks;
        }

        public override void Update(GameTime realTime)
        {
            base.Update(realTime);
        }

        protected override void DoStep(Step step)
        {
            base.DoStep(step);

            this.timeSinceStep = TimeSpan.Zero;
        }

        protected override bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step)
        {
            if (this.stepsSinceTick > this.stepsPerTick)
            {
                throw new Exception();
            }

            if (this.stepsSinceTick == this.stepsPerTick)
            {
                step = null;
                return false;
            }

            if (this.timeSinceStep < this.stepTimeSpan && _ticks.Count == 0)
            {
                step = null;
                return false;
            }

            this.step.TotalTime += this.step.ElapsedTime;
            step = this.step;
            return true;
        }

        protected override bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next)
        {
            if (this.stepsSinceTick > this.stepsPerTick)
            {
                throw new Exception();
            }

            if (this.stepsSinceTick < this.stepsPerTick)
            {
                next = null;
                return false;
            }

            return _ticks.TryPop(current.Id + 1, out next);
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
