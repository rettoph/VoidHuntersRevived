using Guppy.Common;
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
using VoidHuntersRevived.Common.Simulations.Enums;
using VoidHuntersRevived.Common.Entities.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class PilotingSystem : BasicSystem,
        ISimulationEventListener<SetPilotingDirection>,
        ISimulationEventListener<SetPilotingTarget>
    {
        private ComponentMapper<Piloting> _pilotings;
        private ComponentMapper<Pilotable> _pilotables;
        private ComponentMapper<Tractoring> _tractorings;
        private ComponentMapper<Tractorable> _tractorables;
        private State _state;
        private readonly NetScope _scope;
        private readonly ITractorService _tractors;
        private readonly IUserPilotMappingService _userPilotMap;
        private readonly ILogger _logger;

        public PilotingSystem(ITractorService tractors, IUserPilotMappingService userPilots, ILogger logger, NetScope scope, State state)
        {
            _tractors = tractors;
            _userPilotMap = userPilots;
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

        public SimulationEventResult Process(ISimulationEvent<SetPilotingDirection> @event)
        {
            if (!_userPilotMap.TryGetPilotKey(@event.SenderId, out ParallelKey pilotKey))
            {
                return SimulationEventResult.Failure;
            }

            if (!@event.Simulation.TryGetEntityId(pilotKey, out int pilotId))
            {
                return SimulationEventResult.Failure;
            }

            if (!_pilotings.TryGet(pilotId, out var piloting))
            {
                return SimulationEventResult.Failure;
            }

            var pilotable = _pilotables.Get(piloting?.Pilotable);

            if (@event.Body.Value && (pilotable.Direction & @event.Body.Which) == 0)
            {
                pilotable.Direction |= @event.Body.Which;
                return SimulationEventResult.Success;
            }

            if (!@event.Body.Value && (pilotable.Direction & @event.Body.Which) != 0)
            {
                pilotable.Direction &= ~@event.Body.Which;
                return SimulationEventResult.Success;
            }

            return SimulationEventResult.Success;
        }

        public SimulationEventResult Process(ISimulationEvent<SetPilotingTarget> @event)
        {
            if (!_userPilotMap.TryGetPilotKey(@event.SenderId, out ParallelKey pilotKey))
            {
                return SimulationEventResult.Failure;
            }

            if (!@event.Simulation.TryGetEntityId(pilotKey, out int pilotId))
            {
                return SimulationEventResult.Failure;
            }

            if (!_pilotings.TryGet(pilotId, out var piloting))
            {
                return SimulationEventResult.Failure;
            }

            var pilotable = _pilotables.Get(piloting.Pilotable);

            pilotable.Aim.Target = @event.Body.Target;

            return SimulationEventResult.Success;
        }
    }
}
