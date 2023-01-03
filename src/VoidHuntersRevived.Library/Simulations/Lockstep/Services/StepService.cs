using Guppy.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Common;
using VoidHuntersRevived.Library.Simulations.Lockstep.Providers;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Services
{
    internal sealed class StepService : IStepService
    {
        private readonly State _state;
        private readonly IStepProvider _provider;
        private readonly ITickService _ticks;

        public StepService(
            State state,
            ITickService ticks,
            IFiltered<IStepProvider> providers)
        {
            _state = state;
            _provider = providers.Instance ?? throw new Exception();
            _ticks = ticks;
        }

        public void Update(GameTime gameTime)
        {
            _provider.Update(gameTime);

            _ticks.TryTick();

            while (_provider.ShouldStep() && _state.TryStep())
            {
                _ticks.TryTick();
            }
        }
    }
}
