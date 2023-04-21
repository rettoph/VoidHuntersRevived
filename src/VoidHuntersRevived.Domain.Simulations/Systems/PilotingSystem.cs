﻿using Guppy.Common;
using Guppy.Network;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Serilog;
using VoidHuntersRevived.Domain.Entities.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class PilotingSystem : BasicSystem,
        ISubscriber<IEvent<SetPilotingDirection>>,
        ISubscriber<IEvent<SetPilotingTarget>>
    {
        private ComponentMapper<Piloting> _pilotings;
        private ComponentMapper<Pilotable> _pilotables;
        private ComponentMapper<Tractoring> _tractorings;
        private ComponentMapper<Tractorable> _tractorables;
        private State _state;
        private readonly NetScope _scope;
        private readonly ITractorService _tractors;
        private readonly ILogger _logger;

        public PilotingSystem(ITractorService tractors, ILogger logger, NetScope scope, State state)
        {
            _tractors = tractors;
            _logger = logger;
            _scope = scope;
            _state = state;
            _pilotings = default!;
            _pilotables = default!;
            _tractorings = default!;
            _tractorables = default!;
        }

        public override void Initialize(World world)
        {
            _pilotings = world.ComponentMapper.GetMapper<Piloting>();
            _pilotables = world.ComponentMapper.GetMapper<Pilotable>();
            _tractorings = world.ComponentMapper.GetMapper<Tractoring>();
            _tractorables = world.ComponentMapper.GetMapper<Tractorable>();
        }

        public void Process(in IEvent<SetPilotingDirection> message)
        {
            if (!message.Target.TryGetEntityId(message.Sender, out int pilotId))
            {
                return;
            }

            if (!_pilotings.TryGet(pilotId, out var piloting))
            {
                return;
            }

            var pilotable = _pilotables.Get(piloting?.Pilotable);

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
            if(!message.Target.TryGetEntityId(message.Sender, out int pilotId))
            {
                return;
            }

            if (!_pilotings.TryGet(pilotId, out var piloting))
            {
                return;
            }

            var pilotable = _pilotables.Get(piloting.Pilotable);

            pilotable.Aim.Target = message.Data.Target;
        }
    }
}
