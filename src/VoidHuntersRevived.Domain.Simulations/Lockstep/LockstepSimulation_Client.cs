﻿using Guppy.Common;
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

        internal readonly TickBuffer _ticks;

        public LockstepSimulation_Client(
            NetScope netScope,
            TickBuffer ticks,
            ISettingProvider settings,
            ILifetimeScope scope) : base(settings, scope)
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
