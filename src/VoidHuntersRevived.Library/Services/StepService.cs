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
        private readonly SimulationState _state;
        private readonly IStepProvider _provider;
        private readonly ITickService _ticks;

        public StepService(
            SimulationState state,
            ITickService ticks,
            IFiltered<IStepProvider> providers)
        {
            _state = state;
            _provider = providers.Instance ?? throw new Exception();
            _ticks = ticks;
        }

        public override void Update(GameTime gameTime)
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
