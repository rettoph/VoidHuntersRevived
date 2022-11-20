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
        private int _lastTickBufferState;
        private TimeSpan _realTimeSinceStep;
        private readonly ITickService _ticks;
        private readonly ISetting<TimeSpan> _stepInterval;
        private readonly ISetting<int> _stepsPerTick;

        public StepRemoteProvider(
            ITickService ticks,
            ISettingProvider settings)
        {
            _ticks = ticks;
            _stepInterval = settings.Get<TimeSpan>(SettingConstants.StepInterval);
            _stepsPerTick = settings.Get<int>(SettingConstants.StepsPerTick);
        }

        public void Update(GameTime gameTime)
        {
            _realTimeSinceStep += gameTime.ElapsedGameTime;

            this.UpdateTargetStep();
        }

        public bool Next()
        {
            if(_ticks.Provider.Status == TickProviderStatus.Historical && _currentStep / _stepsPerTick .Value < _ticks.Provider.AvailableId)
            {
                _currentStep++;
                _realTimeSinceStep = TimeSpan.Zero;
                return true;
            }

            // There is a constant flux between target step and current step.
            // The 'real time' delay is calculated based on the offset between
            // the target and current. The game time constant is then multiplied
            // by the calculated offset. This slightly changes the real world
            // step delay every time this method is called, so it is constantly
            // chasing the target step.
            float offset = _currentStep - _targetStep;
            var multiplier = this.RealTimeIntervalMultiplier(offset);
            var interval = _stepInterval.Value * multiplier;

            if (_realTimeSinceStep > interval)
            {
                if(_currentStep == _maximumTargetStep)
                {
                    return false;
                }

                _currentStep++;
                _realTimeSinceStep -= interval;

                return true;
            }

            return false;
        }

        private float RealTimeIntervalMultiplier(float offset)
        {
            float amount = ((offset / _stepsPerTick.Value) * 0.25f) + 0.5f;
            float result = MathHelper.SmoothStep(0.75f, 200f, amount);

            return result;
        }

        /// <summary>
        /// Every step we will sychronize the target step.
        /// This is simply done by checking the tick buffer
        /// for the last cached in-order id calculating the 
        /// would be step for that tick.
        /// </summary>
        private void UpdateTargetStep()
        {
            if(_lastTickBufferState == _ticks.Provider.AvailableId)
            {
                return;
            }

            _lastTickBufferState = _ticks.Provider.AvailableId;
            _targetStep = _lastTickBufferState * _stepsPerTick.Value;
            _maximumTargetStep = _targetStep + _stepsPerTick.Value - 1;
        }
    }
}
