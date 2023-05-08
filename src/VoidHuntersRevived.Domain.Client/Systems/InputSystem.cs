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
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities.Tractoring.Components;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    [GuppyFilter<LocalGameGuppy>()]
    [SimulationTypeFilter(SimulationType.Predictive)]
    internal sealed class InputSystem : UpdateSystem,
        ISubscriber<SetPilotingDirection>,
        ISubscriber<TryStartTractoring>,
        ISubscriber<TryStopTractoring>,
        ISubscriber<PreTick>
    {
        private readonly NetScope _netScope;
        private readonly ISimulationService _simulations;
        private readonly IUserPilotMappingService _userPilots;
        private readonly Camera2D _camera;
        private ISimulation _interactive;
        private ComponentMapper<Piloting> _pilotings;
        private ComponentMapper<Pilotable> _pilotables;
        private ComponentMapper<TractorBeamEmitter> _tractorBeamEmitters;
        private ComponentMapper<Parallelable> _parallelables;
        private float _scroll;

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        public InputSystem(
            NetScope netScope,
            Camera2D camera,
            ISimulationService simulations,
            IUserPilotMappingService userPilots)
        {
            _netScope = netScope;
            _camera = camera;
            _simulations = simulations;
            _userPilots = userPilots;
            _interactive = default!;
            _pilotings = default!;
            _pilotables = default!;
            _tractorBeamEmitters = default!;
            _parallelables = default!;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _interactive = _simulations.First(SimulationType.Predictive, SimulationType.Lockstep);

            _pilotings = world.ComponentMapper.GetMapper<Piloting>();
            _pilotables = world.ComponentMapper.GetMapper<Pilotable>();
            _tractorBeamEmitters = world.ComponentMapper.GetMapper<TractorBeamEmitter>();
            _parallelables = world.ComponentMapper.GetMapper<Parallelable>();
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

            _simulations.Enqueue(new SimulationEventData()
            {
                Key = ParallelKey.NewKey(),
                SenderId = _netScope.Peer.Users.Current.Id,
                Body = new SetPilotingTarget()
                {
                    Target = CurrentTargetPosition
                }
            });
        }

        public void Process(in SetPilotingDirection message)
        {
            if (_netScope.Peer?.Users.Current is null)
            {
                return;
            }

            _simulations.Enqueue(new SimulationEventData()
            {
                Key = ParallelKey.NewKey(),
                SenderId = _netScope.Peer.Users.Current.Id,
                Body = new SetPilotingDirection()
                {
                    Which = message.Which,
                    Value = message.Value
                }
            });
        }

        public void Process(in TryStartTractoring message)
        {
            if (_netScope.Peer?.Users.Current is null)
            {
                return;
            }

            if(!_userPilots.TryGetPilotKey(_netScope.Peer.Users.Current.Id, out ParallelKey parallelKey))
            {
                return;
            }

            if(!_interactive.TryGetEntityId(parallelKey, out int pilotId))
            {
                return;
            }

            if(!_pilotings.TryGet(pilotId, out Piloting? piloting))
            {
                return;
            }

            if (!_parallelables.TryGet(piloting.Pilotable.Id, out Parallelable? parallelable))
            {
                return;
            }

            _simulations.Enqueue(new SimulationEventData()
            {
                Key = ParallelKey.NewKey(),
                SenderId = _netScope.Peer.Users.Current.Id,
                Body = new TryStartTractoring()
                {
                    EmitterKey = parallelable.Key
                }
            });
        }

        public void Process(in TryStopTractoring message)
        {
            if (_netScope.Peer?.Users.Current is null)
            {
                return;
            }

            if (!_userPilots.TryGetPilotKey(_netScope.Peer.Users.Current.Id, out ParallelKey parallelKey))
            {
                return;
            }

            if (!_interactive.TryGetEntityId(parallelKey, out int pilotId))
            {
                return;
            }

            if (!_pilotings.TryGet(pilotId, out Piloting? piloting))
            {
                return;
            }

            if(!_parallelables.TryGet(piloting.Pilotable.Id, out Parallelable? parallelable))
            {
                return;
            }

            _simulations.Enqueue(new SimulationEventData()
            {
                Key = ParallelKey.NewKey(),
                SenderId = _netScope.Peer.Users.Current.Id,
                Body = new TryStopTractoring()
                {
                    EmitterKey = parallelable.Key
                }
            });
        }
    }
}
