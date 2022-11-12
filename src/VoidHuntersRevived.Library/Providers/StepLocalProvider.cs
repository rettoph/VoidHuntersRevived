using Guppy.MonoGame.Utilities;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Constants;

namespace VoidHuntersRevived.Library.Providers
{
    internal sealed class StepLocalProvider : IStepProvider
    {
        private readonly GuppyTimer _timer;

        public StepLocalProvider(ISettingProvider settings)
        {
            _timer = new GuppyTimer(settings.Get<TimeSpan>(SettingConstants.StepInterval).Value);
        }

        public void Update(GameTime gameTime)
        {
            _timer.Update(gameTime);
        }

        public bool Ready()
        {
            return _timer.Step(out _);
        }
    }
}
