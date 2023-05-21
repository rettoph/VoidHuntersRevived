using Guppy.Common;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Systems;
using static VoidHuntersRevived.Domain.Simulations.Commands.State;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class CommandSystem : BasicSystem,
        ISubscriber<Ticks>
    {
        private readonly State _state;

        public CommandSystem(State state)
        {
            _state = state;
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
