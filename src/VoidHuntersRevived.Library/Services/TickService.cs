using Guppy.Attributes;
using Guppy.Common;
using Guppy.MonoGame.Utilities;
using Guppy.Network.Enums;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Providers;

namespace VoidHuntersRevived.Library.Services
{
    internal sealed class TickService : ITickService
    {
        private readonly SimulationState _state;
        private readonly ITickProvider _provider;

        public int AvailableId => _provider.AvailableId;

        public TickService(
            SimulationState state,
            IFiltered<ITickProvider> providers)
        {
            _state = state;
            _provider = providers.Instance ?? throw new Exception();
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
