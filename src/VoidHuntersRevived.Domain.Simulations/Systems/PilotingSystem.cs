﻿using Guppy.Common;
using Guppy.Network;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Serilog;
using VoidHuntersRevived.Domain.Entities.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Entities.Components;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class PilotingSystem : BasicSystem,
        ISubscriber<IEvent<SetPilotingDirection>>,
        ISubscriber<IEvent<SetPilotingTarget>>
    {
        private ComponentMapper<Piloting> _pilotings;
        private State _state;
        private NetScope _scope;
        private ILogger _logger;

        public PilotingSystem(ILogger logger, NetScope scope, State state)
        {
            _logger = logger;
            _scope = scope;
            _state = state;
            _pilotings = default!;
        }

        public override void Initialize(World world)
        {
            _pilotings= world.ComponentMapper.GetMapper<Piloting>();
        }

        public void Process(in IEvent<SetPilotingDirection> message)
        {
            var pilotId = message.Simulation.GetEntityId(message.Data.PilotKey);
            var piloting = _pilotings.Get(pilotId);
            var pilotable = piloting.Pilotable;

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

        public void Process(in IEvent<SetPilotingTarget> message)
        {
            if(!message.Simulation.TryGetEntityId(message.Data.PilotKey, out int pilotId))
            {
                return;
            }

            var piloting = _pilotings.Get(pilotId);
            var pilotable = piloting.Pilotable;

            pilotable.Aim.Target = message.Data.Target;
        }
    }
}
