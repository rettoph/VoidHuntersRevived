using Guppy.Common;
using Guppy.Common.Providers;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerTypeFilter(PeerType.Client)]
    internal sealed class LockstepSimulation_Client : LockstepSimulation,
        IDisposable
    {
        private readonly NetScope _scope;
        private readonly TickBuffer _ticks;
        private readonly int _stepsPerTick;
        private int _stepsSinceTick;
        private Step _step;

        public LockstepSimulation_Client(
            NetScope scope,
            TickBuffer ticks,
            ISettingProvider settings, 
            ISpaceFactory spaceFactory,
            IFilteredProvider filtered,
            IBus bus) : base(spaceFactory, filtered, bus)
        {
            Fix64 stepInterval = settings.Get<Fix64>(Settings.StepInterval).Value;

            _scope = scope;
            _ticks = ticks;
            _stepsSinceTick = 0;
            _stepsPerTick = settings.Get<int>(Settings.StepsPerTick).Value;
            _step = new Step()
            {
                ElapsedTime = stepInterval,
                TotalTime = stepInterval
            };
        }

        protected override bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step)
        {
            if (_stepsSinceTick > _stepsPerTick)
            {
                throw new Exception();
            }

            if (_stepsSinceTick == _stepsPerTick)
            {
                step = null;
                return false;
            }

            _step.TotalTime += _step.ElapsedTime;
            step = _step;
            return true;
        }

        protected override void DoStep(Step step)
        {
            base.DoStep(step);

            _stepsSinceTick++;
        }

        protected override bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next)
        {
            if (_stepsSinceTick > _stepsPerTick)
            {
                throw new Exception();
            }

            if (_stepsSinceTick < _stepsPerTick)
            {
                next = null;
                return false;
            }

            return _ticks.TryPop(current.Id + 1, out next);
        }

        protected override void DoTick(Tick tick)
        {
            base.DoTick(tick);

            _stepsSinceTick = 0;
        }

        public override void Enqueue(EventDto data)
        {
            if(data.Data is not IInputData)
            {
                throw new InvalidOperationException();
            }

            _scope.Messages.Create(in data).Enqueue();
        }
    }
}
