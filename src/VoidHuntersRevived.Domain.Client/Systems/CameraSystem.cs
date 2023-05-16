using Guppy.Attributes;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Network;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    [GuppyFilter<LocalGameGuppy>()]
    internal sealed class CameraSystem : MonoGame.Extended.Entities.Systems.DrawSystem
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
            _camera.TargetPosition = Vector2.Zero;
            _camera.TargetVelocity = Vector2.Zero;
        }
    }
}
