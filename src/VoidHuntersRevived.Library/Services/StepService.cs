using Guppy.Attributes;
using Guppy.Common;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Providers;

namespace VoidHuntersRevived.Library.Services
{
    [GuppyFilter(typeof(GameGuppy))]
    internal sealed class StepService : SimpleGameComponent, IStepService
    {
        private readonly IStepProvider _provider;
        private readonly ITickService _ticks;
        private readonly IBus _bus;
        private readonly ISetting<TimeSpan> _stepInterval;
        private readonly ISetting<int> _stepsPerTick;
        private readonly Step _step;
        private int _stepsSinceTick;

        public StepService(
            IBus bus,
            ISettingProvider settings,
            ITickService ticks,
            IFiltered<IStepProvider> providers)
        {
            _bus = bus;
            _provider = providers.Instance ?? throw new Exception();
            _ticks = ticks;
            _stepInterval = settings.Get<TimeSpan>(SettingConstants.StepInterval);
            _stepsPerTick = settings.Get<int>(SettingConstants.StepsPerTick);
            _step = new Step(_stepInterval.Value);
        }

        public override void Update(GameTime gameTime)
        {
            _provider.Update(gameTime);

            this.TryTick();

            while (_provider.Next())
            {
                _bus.Publish(_step);
                _stepsSinceTick++;

                if (this.TryTick() == false)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// False means <see cref="ITickService.Next"/> returned false.
        /// True means that it either returned true, or did not run at all
        /// </summary>
        /// <returns></returns>
        private bool TryTick()
        {
            if (_stepsSinceTick < _stepsPerTick.Value)
            {
                return false;
            }

            if (_ticks.Next())
            {
                _stepsSinceTick = 0;
                return true;
            }

            return true;
        }
    }
}
