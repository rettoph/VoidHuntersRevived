using Guppy.Common;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Factories;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Providers
{
    [PeerTypeFilter(PeerType.Server | PeerType.None)]
    internal sealed class DefaultTickProvider : ITickProvider
    {
        private readonly State _state;
        private readonly ITickFactory _factory;

        public int AvailableId => _state.NextTickId;


        public DefaultTickProvider(State state, IFiltered<ITickFactory> factory)
        {
            _state = state;
            _factory = factory.Instance;
        }

        public bool TryGetNextTick([MaybeNullWhen(false)] out Tick next)
        {
            next = _factory.Create(_state.NextTickId);

            return true;
        }
    }
}
