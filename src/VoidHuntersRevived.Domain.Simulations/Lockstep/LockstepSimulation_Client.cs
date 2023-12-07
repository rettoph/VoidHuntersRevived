using Guppy.Common;
using Guppy.Common.Providers;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using Autofac;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerFilter(PeerType.Client)]
    internal sealed class LockstepSimulation_Client : LockstepSimulation,
        IDisposable
    {
        private readonly NetScope _netScope;
        private Step _step;

        internal readonly TickBuffer _ticks;
        internal readonly int _stepsPerTick;
        internal int _stepsSinceTick;
        private TimeSpan _timeSinceStep;
        private TimeSpan _stepTimeSpan;

        public LockstepSimulation_Client(
            NetScope netScope,
            TickBuffer ticks,
            ISettingProvider settings,
            ILifetimeScope scope) : base(scope)
        {
            Fix64 stepInterval = settings.Get(Settings.StepInterval).Value;

            _netScope = netScope;
            _ticks = ticks;
            _stepsSinceTick = 0;
            _stepsPerTick = settings.Get(Settings.StepsPerTick).Value;
            _stepTimeSpan = TimeSpan.FromSeconds((float)stepInterval);
            _step = new Step()
            {
                ElapsedTime = stepInterval,
                TotalTime = stepInterval
            };
        }

        public override void Update(GameTime realTime)
        {
            _timeSinceStep += realTime.ElapsedGameTime;

            base.Update(realTime);
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

            if (_timeSinceStep < _stepTimeSpan && _ticks.Count == 0)
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
            _timeSinceStep -= _stepTimeSpan;
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

        public override void Input(VhId sender, IInputData data)
        {
            _netScope.Messages.Create(new EventDto()
            {
                Sender = sender,
                Data = data
            }).Enqueue();
        }
    }
}
