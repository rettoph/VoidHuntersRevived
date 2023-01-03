using Guppy.Common;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Common;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Simulations.Lockstep.Services;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Providers
{
    [PeerTypeFilter(PeerType.Client)]
    internal sealed class ClientStepProvider : IStepProvider
    {
        private const double TargetStrength = 0.02f;
        private const double IntervalStrength = 0.01f;

        private int _targetStep;
        private int _maximumTargetStep;
        private int _lastAvailableTickId;
        private TimeSpan _realTimeSinceStep;
        private TimeSpan _currentInterval;
        private TimeSpan _targetInterval;
        private readonly TimeSpan _interval;
        private readonly int _stepsPerTick;
        private readonly State _state;
        private readonly ITickProvider _ticks;

        public TimeSpan CurrentInterval => _currentInterval;
        public TimeSpan TargetInterval => _targetInterval;

        public int TargetStep => _targetStep;

        public ClientStepProvider(
            State state,
            IFiltered<ITickProvider> ticks,
            ISettingProvider settings)
        {
            _state = state;
            _ticks = ticks.Instance ?? throw new ArgumentNullException();
            _interval = settings.Get<TimeSpan>(SettingConstants.StepInterval).Value;
            _stepsPerTick = settings.Get<int>(SettingConstants.StepsPerTick).Value;

            _currentInterval = _interval;
            _targetInterval = _interval;
        }

        public void Update(GameTime gameTime)
        {
            if (_realTimeSinceStep < _currentInterval)
            {
                _realTimeSinceStep += gameTime.ElapsedGameTime;
            }

            this.UpdateTarget();
        }

        public bool ShouldStep()
        {
            if (_state.Reading)
            {
                return false;
            }

            if (_realTimeSinceStep >= _currentInterval)
            {
                if (_state.LastStep == _maximumTargetStep)
                {
                    return false;
                }

                _realTimeSinceStep -= _currentInterval;
                this.UpdateInterval();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Every update we will sychronize the target step.
        /// This is simply done by checking the tick buffer
        /// for the last cached in-order id calculating the 
        /// would be step for that tick.
        /// </summary>
        private void UpdateTarget()
        {
            if (_lastAvailableTickId == _ticks.AvailableId)
            {
                return;
            }

            _lastAvailableTickId = _ticks.AvailableId;
            _targetStep = _lastAvailableTickId * _stepsPerTick;
            _maximumTargetStep = _targetStep + _stepsPerTick - 1;
        }

        /// <summary>
        /// There is a constant flux between target step and current step.
        /// The 'real time' delay is calculated based on the offset between
        /// the target and current. The game time constant is then multiplied
        /// by the calculated offset. This slightly changes the real world
        /// step delay every time this method is called, so it is constantly
        /// chasing the target step.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="currentStep"></param>
        /// <param name="targetStep"></param>
        /// <param name="interval"></param>
        private void UpdateInterval()
        {
            if (_state.LastStep == _targetStep)
            {
                return;
            }

            float multiplier = _targetStep - _state.LastStep;
            multiplier = _stepsPerTick - multiplier;
            multiplier /= _stepsPerTick;
            multiplier = Math.Clamp(multiplier, 0.25f, 4f);

            _targetInterval = _interval * multiplier;

            // "lerp" towards the target
            TimeSpan difference = _targetInterval - _currentInterval;
            difference *= TargetStrength;
            _currentInterval += difference;

            // "lerp" towards the rigid interval
            difference = _interval - _currentInterval;
            difference *= IntervalStrength;
            _currentInterval += difference;
        }
    }
}
