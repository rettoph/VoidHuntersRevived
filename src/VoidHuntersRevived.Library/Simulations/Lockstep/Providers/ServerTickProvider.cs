using Guppy.Common;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Library.Simulations.Lockstep.Factories;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Providers
{
    [PeerTypeFilter(PeerType.Server)]
    internal sealed class ServerTickProvider : ITickProvider
    {
        private readonly State _state;
        private readonly ITickFactory _factory;

        public int AvailableId => _state.NextTickId;


        public ServerTickProvider(State state, IFiltered<ITickFactory> factory)
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
