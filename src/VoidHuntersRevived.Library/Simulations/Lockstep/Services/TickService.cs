using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Common;
using VoidHuntersRevived.Library.Simulations.Lockstep.Providers;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Services
{
    internal sealed class TickService : ITickService
    {
        private readonly State _state;
        private readonly ITickProvider _provider;

        public TickService(
            State state,
            IFiltered<ITickProvider> providers)
        {
            _state = state;
            _provider = providers.Instance ?? throw new ArgumentNullException();
        }

        public bool TryTick()
        {
            if (_state.CanTick() && _provider.TryGetNextTick(out var next))
            {
                _state.TryTick(next);

                return true;
            }

            return false;
        }
    }
}
