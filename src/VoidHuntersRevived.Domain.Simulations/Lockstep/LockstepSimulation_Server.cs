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
using VoidHuntersRevived.Common.Simulations.Factories;
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerTypeFilter(PeerType.Server)]
    internal sealed class LockstepSimulation_Server : LockstepSimulation
    {
        private readonly List<EventDto> _events;
        private readonly NetScope _scope;
        private readonly int _stepsPerTick;
        private int _stepsSinceTick;
        private TimeSpan _timeSinceStep;
        private TimeSpan _stepTimeSpan;
        private Step _step;

        public LockstepSimulation_Server(
            NetScope scope,
            ISettingProvider settings, 
            IWorldFactory worldFactory, 
            ISpaceFactory spaceFactory) : base(worldFactory, spaceFactory)
        {
            Fix64 stepInterval = settings.Get<Fix64>(Settings.StepInterval).Value;

            _scope = scope;
            _stepsSinceTick = 0;
            _events = new List<EventDto>();
            _stepsPerTick = settings.Get<int>(Settings.StepsPerTick).Value;
            _stepTimeSpan = TimeSpan.FromMilliseconds((int)stepInterval);
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
            if (_stepsSinceTick >= _stepsPerTick)
            {
                step = null;
                return false;
            }

            if(_timeSinceStep < _stepTimeSpan)
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

        public override void Enqueue(EventDto data)
        {
            _events.Add(data);
        }

        protected override bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next)
        {
            if(_stepsSinceTick != _stepsPerTick)
            {
                next = null;
                return false;
            }

            next = current.Next(_events.ToArray());
            _events.Clear();

            return true;
        }

        protected override void DoTick(Tick tick)
        {
            base.DoTick(tick);

            // Broadcast the current tick to all connected peers
            _scope.Messages.Create(in tick)
                .AddRecipients(_scope.Users.Peers)
                .Enqueue();

            _stepsSinceTick = 0;
        }
    }
}
