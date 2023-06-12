using Guppy.Attributes;
using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Game.Client.Messages;
using VoidHuntersRevived.Game.Common.Events;

namespace VoidHuntersRevived.Game.Client.Systems
{
    [AutoLoad]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class InputSystem : BasicSystem,
        ISubscriber<SetHelmDirection>
    {
        private readonly ISimulationService _simulations;

        public InputSystem(ISimulationService simulations)
        {
            _simulations = simulations;
        }

        public void Process(in SetHelmDirection message)
        {
            _simulations.Enqueue(message);
        }
    }
}
