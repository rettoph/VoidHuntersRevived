using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Collections;
using Guppy.GUI;
using Guppy.GUI.Elements;
using Guppy.GUI.Providers;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Serilog;
using Serilog.Core;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Client.Messages.Commands;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    [GuppyFilter<LocalGameGuppy>()]
    internal sealed class CameraSystem : MonoGame.Extended.Entities.Systems.DrawSystem,
        ISubscriber<Zoom>
    {
        private readonly Camera2D _camera;
        private readonly NetScope _scope;
        private readonly ISimulationService _simulations;
        private ComponentMapper<IBody> _bodies;
        private ILogger _logger;
        private Stage _stage;
        private Label _fps;
        private TimeSpan _timeSinceSecond;
        private float _framesSinceSecond;
        private Buffer<float> _framesPerSecond = new Buffer<float>(10);
        private TimeSpan _second = TimeSpan.FromSeconds(1);

        public CameraSystem(
            IStageProvider stages,
            ILogger logger, 
            NetScope scope, 
            Camera2D camera, 
            ISimulationService simulations)
        {
            _stage = stages.Create();
            _logger = logger;
            _camera = camera;
            _scope = scope;
            _simulations = simulations;
            _bodies = default!;

            _camera.Zoom = 100;

            _fps = new Label()
            {
                Color = Color.Red,
                Text = ""
            };

            _stage.Add(_fps);
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _bodies = world.ComponentManager.GetMapper<IBody>();
        }

        public override void Draw(GameTime gameTime)
        {
            this.UpdateCameraTargets(gameTime);

            _timeSinceSecond += gameTime.ElapsedGameTime;
            _framesSinceSecond++;

            if(_timeSinceSecond >= _second)
            {
                _timeSinceSecond -= _second;
                _framesPerSecond.Add(_framesSinceSecond);
                _framesSinceSecond = 0;

                _fps.Text = _framesPerSecond.Average().ToString("#,##0.00");
            }

            _camera.Update(gameTime);
            _stage.Draw(gameTime);
        }

        private void UpdateCameraTargets(GameTime gameTime)
        {
            _camera.TargetPosition = Vector2.Zero;
            _camera.TargetVelocity = Vector2.Zero;
        }

        public void Process(in Zoom message)
        {
            if(message.Target is null)
            {
                _logger.Information($"Current Zoom Target: {_camera.TargetZoom}");
                return;

            }
            _camera.TargetZoom = message.Target.Value;
        }
    }
}
