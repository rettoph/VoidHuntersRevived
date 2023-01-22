using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations.Systems;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using Microsoft.Xna.Framework.Input;
using Guppy.MonoGame.Utilities.Cameras;
using MonoGame.Extended;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep.Messages;
using Guppy.Network.Identity;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using Guppy.Attributes;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    [GuppyFilter<ClientGameGuppy>()]
    [SimulationTypeFilter(SimulationType.Predictive)]
    internal sealed class InputSystem : UpdateSystem,
        ISubscriber<SetPilotingDirection>,
        ISubscriber<StartTractoring>,
        ISubscriber<PreTick>
    {
        private readonly NetScope _netScope;
        private readonly ISimulationService _simulations;
        private readonly Camera2D _camera;
        private readonly ITractorService _tractor;
        private ComponentMapper<Piloting> _pilotings;
        private ComponentMapper<Pilotable> _pilotables;

        private Vector2 CurrentTarget => _camera.Unproject(Mouse.GetState().Position.ToVector2());
        private ParallelKey CurrentPilotKey
        {
            get
            {
                if (_netScope.Peer?.Users.Current is null)
                {
                    return default;
                }

                return ParallelKey.From(ParallelTypes.Pilot, _netScope.Peer.Users.Current.Id);
            }
        }

        public InputSystem(
            NetScope netScope,
            Camera2D camera,
            ISimulationService simulations,
            ITractorService tractor)
        {
            _netScope = netScope;
            _camera = camera;
            _simulations = simulations;
            _tractor = tractor;

            _pilotings = default!;
            _pilotables = default!;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _pilotings = world.ComponentMapper.GetMapper<Piloting>();
            _pilotables = world.ComponentMapper.GetMapper<Pilotable>();
        }

        public override void Update(GameTime gameTime)
        {
            this.SetTarget(SimulationType.Predictive);
        }

        public void Process(in PreTick message)
        {
            this.SetTarget(SimulationType.Lockstep);
        }

        public void Process(in SetPilotingDirection message)
        {
            _simulations.Input(
                user: CurrentPilotKey,
                data: new SetPilotingDirection(
                    which: message.Which,
                    value: message.Value));
        }

        public void Process(in StartTractoring message)
        {
            if (_tractor.TryGetTractorable(this.CurrentTarget, out var tractorableKey))
            {
                _simulations.Input(
                    user: CurrentPilotKey,
                    data: new StartTractoring()
                    {
                        Tractorable = tractorableKey
                    });
            }
        }

        private void SetTarget(SimulationType simulation)
        {
            _simulations[simulation].Input(
                user: CurrentPilotKey,
                data: new SetPilotingTarget()
                {
                    Target = CurrentTarget
                });
        }
    }
}
