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
    internal sealed class StepService : SimpleGameComponent, IStepService
    {
        private readonly IStepProvider _provider;
        private readonly IBus _bus;
        private readonly ISetting<TimeSpan> _stepInterval;
        private readonly Step _step;

        public StepService(
            IBus bus,
            ISettingProvider settings,
            IFiltered<IStepProvider> providers)
        {
            _bus = bus;
            _provider = providers.Instance ?? throw new Exception();
            _stepInterval = settings.Get<TimeSpan>(SettingConstants.StepInterval);
            _step = new Step(_stepInterval.Value);
        }

        public override void Update(GameTime gameTime)
        {
            _provider.Update(gameTime);

            while(_provider.Ready())
            {
                _bus.Publish(_step);
            }
        }
    }
}
