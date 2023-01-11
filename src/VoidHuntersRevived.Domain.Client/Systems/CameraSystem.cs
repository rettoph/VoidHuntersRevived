using Guppy.Attributes;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Network;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    [GuppyFilter<ClientGameGuppy>()]
    internal sealed class CameraSystem : DrawSystem
    {
        private readonly Camera2D _camera;
        private readonly NetScope _scope;
        private readonly ISimulationService _simulations;
        private ComponentMapper<Body> _bodies;

        public CameraSystem(NetScope scope, Camera2D camera, ISimulationService simulations)
        {
            _camera = camera;
            _scope = scope;
            _simulations = simulations;
            _bodies = default!;

            _camera.Zoom = 100;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _bodies = world.ComponentMapper.GetMapper<Body>();
        }

        public override void Draw(GameTime gameTime)
        {
            this.UpdateCameraTargets(gameTime);

            _camera.Update(gameTime);
        }

        private void UpdateCameraTargets(GameTime gameTime)
        {
            if(!_simulations.Flags.HasFlag(SimulationType.Predictive))
            {
                return;
            }

            var currentUserId = _scope.Peer!.Users.Current?.Id;
            if (currentUserId is null)
            {
                return;
            }

            if (!_simulations[SimulationType.Predictive].TryGetEntityId(new ParallelKey(ParallelTypes.Ship, currentUserId.Value), out var shipEntityId))
            {
                return;
            }

            var body = _bodies.Get(shipEntityId);
            if (body is null)
            {
                return;
            }

            _camera.TargetPosition = body.WorldCenter;
            _camera.TargetVelocity = body.LinearVelocity;
        }
    }
}
