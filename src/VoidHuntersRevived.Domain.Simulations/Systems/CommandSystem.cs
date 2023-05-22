using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Systems;
using static VoidHuntersRevived.Domain.Simulations.Commands.State;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [PeerTypeFilter(PeerType.Client)]
    [GuppyFilter<IGameGuppy>]
    internal sealed class CommandSystem : BasicSystem,
        ISubscriber<Ticks>
    {
        private readonly IState _state;

        public CommandSystem(IFiltered<IState> state)
        {
            _state = state.Instance;
        }

        public void Process(in Ticks message)
        {
            foreach(Tick tick in _state.History)
            {
                Console.WriteLine($"{tick.Id} - {tick.Hash().Value}");
            }
        }
    }
}
