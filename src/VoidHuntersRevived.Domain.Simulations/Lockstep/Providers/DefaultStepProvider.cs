using Guppy.MonoGame.Utilities;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Providers
{
    [PeerTypeFilter(PeerType.Server | PeerType.None)]
    internal sealed class DefaultStepProvider : IStepProvider
    {
        private readonly GuppyTimer _timer;

        public DefaultStepProvider(ISettingProvider settings)
        {
            _timer = new GuppyTimer(settings.Get<TimeSpan>(Settings.StepInterval).Value);
        }

        public void Update(GameTime gameTime)
        {
            _timer.Update(gameTime);
        }

        public bool ShouldStep()
        {
            return _timer.Step(out _);
        }
    }
}
