using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common.PhysicsLogic;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Providers
{
    internal sealed class StepRemoteProvider : IStepProvider
    {
        private int _currentStep;
        private int _targetStep;
        private int _maximumTargetStep;
        private int _lastAvailableTickId;
        private TimeSpan _realTimeSinceStep;
        private TimeSpan _currentInterval;
        private TimeSpan _targetInterval;
        private readonly TimeSpan _currentDelta;
        private readonly TimeSpan _targetDelta;
        private readonly TimeSpan _interval;
        private readonly int _stepsPerTick;
        private readonly ITickService _ticks;

        private int CurrenTickId => _currentStep / _stepsPerTick;

        public TimeSpan CurrentInterval => _currentInterval;
        public TimeSpan TargetInterval => _targetInterval;

        public StepRemoteProvider(
            ITickService ticks,
            ISettingProvider settings)
        {
            _ticks = ticks;
            _interval = settings.Get<TimeSpan>(SettingConstants.StepInterval).Value;
            _stepsPerTick = settings.Get<int>(SettingConstants.StepsPerTick).Value;

            _currentInterval = _interval;
            _targetInterval = _interval;
            _currentDelta = _interval * 0.01f;
            _targetDelta = _interval * 0.5f;
        }

        public void Update(GameTime gameTime)
        {
            _realTimeSinceStep += gameTime.ElapsedGameTime;

            this.UpdateTarget();
        }

        public bool Next()
        {
            if(_ticks.Provider.Status == TickProviderStatus.Historical && this.CurrenTickId < _lastAvailableTickId)
            {
                _currentStep++;
                _realTimeSinceStep = TimeSpan.Zero;
                return true;
            }

            if (_realTimeSinceStep >= _currentInterval)
            {
                if(_currentStep == _maximumTargetStep)
                {
                    return false;
                }

                _currentStep++;
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
            if(_lastAvailableTickId == _ticks.Provider.AvailableId)
            {
                return;
            }

            _lastAvailableTickId = _ticks.Provider.AvailableId;
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
            if(_currentStep == _targetStep)
            {
                return;
            }

            float multiplier = _currentStep - _targetStep;
            multiplier /= _stepsPerTick;
            multiplier = Math.Clamp(multiplier, -1f, 1f);

            _targetInterval = _targetDelta * multiplier;
            _targetInterval += _interval;

            if(_currentInterval < _targetInterval)
            {
                _currentInterval += _currentDelta;
                return;
            }

            if (_currentInterval > _targetInterval)
            {
                _currentInterval -= _currentDelta;
                return;
            }
        }
    }
}
