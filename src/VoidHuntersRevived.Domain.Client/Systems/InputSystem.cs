using Guppy.Common;
using Guppy.Network;
using VoidHuntersRevived.Domain.Entities.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using Microsoft.Xna.Framework.Input;
using Guppy.MonoGame.Utilities.Cameras;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep.Messages;
using Guppy.Attributes;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    [GuppyFilter<LocalGameGuppy>()]
    [SimulationTypeFilter(SimulationType.Predictive)]
    internal sealed class InputSystem : UpdateSystem,
        ISubscriber<SetHelmDirection>,
        ISubscriber<ActivateTractorBeamEmitter>,
        ISubscriber<DeactivateTractorBeamEmitter>,
        ISubscriber<PreTick>
    {
        private readonly NetScope _netScope;
        private readonly ISimulationService _simulations;
        private readonly IUserShipMappingService _userShips;
        private readonly Camera2D _camera;
        private ISimulation _interactive;
        private ComponentMapper<Parallelable> _parallelables;
        private float _scroll;

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        public InputSystem(
            NetScope netScope,
            Camera2D camera,
            ISimulationService simulations,
            IUserShipMappingService userShips)
        {
            _netScope = netScope;
            _camera = camera;
            _simulations = simulations;
            _userShips = userShips;
            _interactive = default!;
            _parallelables = default!;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _interactive = _simulations.First(SimulationType.Predictive, SimulationType.Lockstep);

            _parallelables = world.ComponentManager.GetMapper<Parallelable>();
        }

        public override void Update(GameTime gameTime)
        {
            var mouseScroll = Mouse.GetState().ScrollWheelValue / 120f;
            var delta = _scroll - mouseScroll;
            _scroll = mouseScroll;

            _camera.TargetZoom *= 1f - (0.1f * delta);
        }

        public void Process(in PreTick message)
        {
            if (_netScope.Peer?.Users.Current is null)
            {
                return;
            }

            if (!_userShips.TryGetShipKey(_netScope.Peer.Users.Current.Id, out ParallelKey shipKey))
            {
                return;
            }

            _simulations.Enqueue(new SimulationEventData()
            {
                Key = ParallelKey.NewKey(),
                SenderId = _netScope.Peer.Users.Current.Id,
                Body = new SetTacticalTarget()
                {
                    TacticalKey = shipKey,
                    Target = CurrentTargetPosition
                }
            });
        }

        public void Process(in SetHelmDirection message)
        {
            if (_netScope.Peer?.Users.Current is null)
            {
                return;
            }

            if(!_userShips.TryGetShipKey(_netScope.Peer.Users.Current.Id, out ParallelKey shipKey))
            {
                return;
            }

            _simulations.Enqueue(new SimulationEventData()
            {
                Key = ParallelKey.NewKey(),
                SenderId = _netScope.Peer.Users.Current.Id,
                Body = new SetHelmDirection()
                {
                    HelmKey = shipKey,
                    Which = message.Which,
                    Value = message.Value
                }
            });
        }

        public void Process(in ActivateTractorBeamEmitter message)
        {
            if (_netScope.Peer?.Users.Current is null)
            {
                return;
            }

            if (!_userShips.TryGetShipKey(_netScope.Peer.Users.Current.Id, out ParallelKey shipKey))
            {
                return;
            }

            _simulations.Enqueue(new SimulationEventData()
            {
                Key = ParallelKey.NewKey(),
                SenderId = _netScope.Peer.Users.Current.Id,
                Body = new ActivateTractorBeamEmitter()
                {
                    TractorBeamEmitterKey = shipKey,
                }
            });
        }

        public void Process(in DeactivateTractorBeamEmitter message)
        {
            if (_netScope.Peer?.Users.Current is null)
            {
                return;
            }

            if (!_userShips.TryGetShipKey(_netScope.Peer.Users.Current.Id, out ParallelKey shipKey))
            {
                return;
            }

            _simulations.Enqueue(new SimulationEventData()
            {
                Key = ParallelKey.NewKey(),
                SenderId = _netScope.Peer.Users.Current.Id,
                Body = new DeactivateTractorBeamEmitter()
                {
                    TractorBeamEmitterKey = shipKey,
                }
            });
        }
    }
}
