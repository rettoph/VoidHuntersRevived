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
    [GuppyFilter<LocalGameGuppy>()]
    [SimulationTypeFilter(SimulationType.Predictive)]
    internal sealed class InputSystem : UpdateSystem,
        ISubscriber<SetPilotingDirection>,
        ISubscriber<StartTractoring>,
        ISubscriber<StopTractoring>,
        ISubscriber<PreTick>
    {
        private readonly NetScope _netScope;
        private readonly ISimulationService _simulations;
        private readonly Camera2D _camera;
        private readonly ITractorService _tractor;
        private ISimulation _interactive;
        private ComponentMapper<Piloting> _pilotings;
        private ComponentMapper<Pilotable> _pilotables;
        private float _scroll;

        private Vector2 CurrentTarget => _camera.Unproject(Mouse.GetState().Position.ToVector2());
        private ParallelKey CurrentUserKey
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
            _interactive = default!;
            _pilotings = default!;
            _pilotables = default!;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _interactive = _simulations.First(SimulationType.Predictive, SimulationType.Lockstep);

            _pilotings = world.ComponentMapper.GetMapper<Piloting>();
            _pilotables = world.ComponentMapper.GetMapper<Pilotable>();
        }

        public override void Update(GameTime gameTime)
        {
            this.SetTarget(SimulationType.Predictive);

            var mouseScroll = Mouse.GetState().ScrollWheelValue / 120f;
            var delta = _scroll - mouseScroll;
            _scroll = mouseScroll;

            if(delta != 0)
            {
                Console.WriteLine(delta);
            }

            _camera.TargetZoom *= 1f - (0.1f * delta);
        }

        public void Process(in PreTick message)
        {
            this.SetTarget(SimulationType.Lockstep);
        }

        public void Process(in SetPilotingDirection message)
        {
            _simulations.Input(
                user: CurrentUserKey,
                data: new SetPilotingDirection(
                    which: message.Which,
                    value: message.Value));
        }

        private void SetTarget(SimulationType simulation)
        {
            _simulations[simulation].Input(
                user: CurrentUserKey,
                data: new SetPilotingTarget()
                {
                    Target = CurrentTarget
                });
        }

        public void Process(in StartTractoring message)
        {
            var pilotId = _interactive.GetEntityId(this.CurrentUserKey);
            var piloting = _pilotings.Get(pilotId);

            if(piloting is null)
            {
                return;
            }

            var pilotable = _pilotables.Get(piloting.Pilotable);

            if (_tractor.TryGetTractorable(pilotable, out var tractorable, out var node))
            {
                _simulations.Input(
                    user: CurrentUserKey,
                    data: new StartTractoring()
                    {
                        Tractorable = tractorable,
                        Node = node
                    });
            }
        }

        public void Process(in StopTractoring message)
        {
            _simulations.Input(
                user: CurrentUserKey,
                data: new StopTractoring()
                {
                    Target = CurrentTarget
                });
        }
    }
}
