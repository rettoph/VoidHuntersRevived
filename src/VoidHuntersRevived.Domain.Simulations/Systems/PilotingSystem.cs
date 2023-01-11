using Guppy.Common;
using Guppy.MonoGame;
using Guppy.Network;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Enums;
using VoidHuntersRevived.Domain.Entities.Messages;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class PilotingSystem : EntitySystem,
        ISubscriber<ISimulationEvent<SetPilotingDirection>>
    {
        private ComponentMapper<Piloting> _pilotings;
        private State _state;
        private NetScope _scope;
        private ILogger _logger;

        public PilotingSystem(ILogger logger, NetScope scope, State state) : base(Aspect.All(typeof(Piloting)))
        {
            _logger = logger;
            _scope = scope;
            _state = state;
            _pilotings = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _pilotings= mapperService.GetMapper<Piloting>();
        }

        public void Process(in ISimulationEvent<SetPilotingDirection> message)
        {
            var pilotId = message.Simulation.GetEntityId(message.Data.PilotKey);
            var piloting = _pilotings.Get(pilotId);
            var pilotable = piloting?.Pilotable;

            if(pilotable is null)
            {
                return;
            }

            if (message.Data.Value && (pilotable.Direction & message.Data.Which) == 0)
            {
                pilotable.Direction |= message.Data.Which;
                return;
            }

            if (!message.Data.Value && (pilotable.Direction & message.Data.Which) != 0)
            {
                pilotable.Direction &= ~message.Data.Which;
                return;
            }
        }
    }
}
