using Guppy.Common;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Providers;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Services
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
            _provider = providers.Instance;
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
