using Autofac;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Attributes;
using Guppy.Core.Network.Common.Enums;
using Microsoft.Xna.Framework;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerFilter(PeerType.Client)]
    public sealed class LockstepStrategy_Client : LockstepStrategy,
        IDisposable
    {
        private readonly INetScope<IStrategy> _netScope;

        internal readonly TickBuffer _ticks;

        public LockstepStrategy_Client(
            INetScope<IStrategy> netScope,
            TickBuffer ticks,
            int stepsPerInterval,
            Fix64 stepInterval,
            Lazy<ISimulation> simulation,
            Lazy<IEngineService> engines,
            Lazy<ILogger> logger) : base(stepsPerInterval, stepInterval, simulation, engines, logger)
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

            if (this.TimeSinceStep < this.StepTimeSpan && _ticks.Count == 0)
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

        public static object Factory(ILifetimeScope scope)
        {
            return new LockstepStrategy_Client(
                scope.Resolve<INetScope<IStrategy>>(),
                scope.Resolve<TickBuffer>(),
                Settings.StepsPerTick.Value,
                Settings.StepInterval.Value,
                scope.Resolve<Lazy<ISimulation>>(),
                scope.Resolve<Lazy<IEngineService>>(),
                scope.Resolve<Lazy<ILogger>>());
        }
    }
}
