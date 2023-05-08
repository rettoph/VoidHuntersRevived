using Guppy.Common;
using Guppy.Network;
using MonoGame.Extended.Entities;
using Serilog;
using VoidHuntersRevived.Domain.Entities.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities.Tractoring.Components;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class PilotingSystem : BasicSystem,
        ISimulationEventListener<SetPilotingDirection>,
        ISimulationEventListener<SetPilotingTarget>
    {
        private ComponentMapper<Piloting> _pilotings;
        private ComponentMapper<Pilotable> _pilotables;
        private State _state;
        private readonly NetScope _scope;
        private readonly IUserPilotMappingService _userPilotMap;
        private readonly ILogger _logger;

        public PilotingSystem(IUserPilotMappingService userPilots, ILogger logger, NetScope scope, State state)
        {
            _userPilotMap = userPilots;
            _logger = logger;
            _scope = scope;
            _state = state;
            _pilotings = default!;
            _pilotables = default!;
        }

        public override void Initialize(World world)
        {
            _pilotings = world.ComponentMapper.GetMapper<Piloting>();
            _pilotables = world.ComponentMapper.GetMapper<Pilotable>();
        }

        public void Process(ISimulationEvent<SetPilotingDirection> @event)
        {
            if (!_userPilotMap.TryGetPilotKey(@event.SenderId, out ParallelKey pilotKey))
            {
                return;
            }

            if (!@event.Simulation.TryGetEntityId(pilotKey, out int pilotId))
            {
                return;
            }

            if (!_pilotings.TryGet(pilotId, out var piloting))
            {
                return;
            }

            var pilotable = _pilotables.Get(piloting?.Pilotable);

            if (@event.Body.Value && (pilotable.Direction & @event.Body.Which) == 0)
            {
                pilotable.Direction |= @event.Body.Which;
                return;
            }

            if (!@event.Body.Value && (pilotable.Direction & @event.Body.Which) != 0)
            {
                pilotable.Direction &= ~@event.Body.Which;
                return;
            }
        }

        public void Process(ISimulationEvent<SetPilotingTarget> @event)
        {
            if (!_userPilotMap.TryGetPilotKey(@event.SenderId, out ParallelKey pilotKey))
            {
                return;
            }

            if (!@event.Simulation.TryGetEntityId(pilotKey, out int pilotId))
            {
                return;
            }

            if (!_pilotings.TryGet(pilotId, out var piloting))
            {
                return;
            }

            var pilotable = _pilotables.Get(piloting.Pilotable);

            pilotable.Aim.Target = @event.Body.Target;
        }
    }
}
