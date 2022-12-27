using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Collections;
using Guppy.MonoGame.Utilities;
using Guppy.Network.Enums;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Providers
{
    internal sealed class TickLocalProvider : ITickProvider
    {
        private readonly SimulationState _state;
        private readonly ITickFactory _factory;

        public int AvailableId => _state.NextTickId;

        public TickProviderStatus Status { get; }

        public TickLocalProvider(SimulationState state, ITickFactory factory)
        {
            _state = state;
            _factory = factory;

            this.Status = TickProviderStatus.Realtime;
        }

        public bool TryGetNextTick([MaybeNullWhen(false)] out Tick next)
        {
            next = _factory.Create(_state.NextTickId);

            return true;
        }
    }
}
