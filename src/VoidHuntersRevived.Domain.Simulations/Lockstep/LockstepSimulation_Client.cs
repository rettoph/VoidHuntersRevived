﻿using Guppy.Common;
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
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerTypeFilter(PeerType.Client)]
    internal sealed class LockstepSimulation_Client : LockstepSimulation,
        IDisposable
    {
        private readonly TickBuffer _ticks;
        private readonly int _stepsPerTick;
        private int _stepsSinceTick;
        private Step _step;

        private List<EventDto> _events;

        public LockstepSimulation_Client(
            TickBuffer ticks,
            ISettingProvider settings, 
            ISpaceFactory spaceFactory,
            IFilteredProvider filtered,
            IBus bus) : base(spaceFactory, filtered, bus)
        {
            _ticks = ticks;
            _stepsSinceTick = 0;
            _stepsPerTick = settings.Get<int>(Settings.StepsPerTick).Value;
            _step = new Step();
            _events = new List<EventDto>();
        }

        public override void Update(GameTime realTime)
        {
            Fix64 fixedElapsedTime = (Fix64)realTime.ElapsedGameTime.TotalSeconds;

            _step.TotalTime += fixedElapsedTime;
            _step.ElapsedTime += fixedElapsedTime;

            base.Update(realTime);
        }

        protected override bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step)
        {
            if (_stepsSinceTick >= _stepsPerTick)
            {
                step = null;
                return false;
            }

            step = _step;
            return true;
        }

        protected override void DoStep(Step step)
        {
            base.DoStep(step);

            _stepsSinceTick++;
            _step.ElapsedTime = Fix64.Zero;
        }

        public override void Enqueue(EventDto data)
        {
            _events.Add(data);
        }

        protected override bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next)
        {
            if (_stepsSinceTick != _stepsPerTick)
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
    }
}
