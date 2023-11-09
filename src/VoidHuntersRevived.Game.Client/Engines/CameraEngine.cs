using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Attributes;
using Guppy.Input.Messages;
using Guppy.MonoGame;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Svelto.ECS;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Enums;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [SimulationTypeFilter(SimulationType.Predictive)]
    [Sequence<DrawEngineSequence>(DrawEngineSequence.PreDraw)]
    internal sealed class CameraEngine : BasicEngine, IStepEngine<GameTime>,
        ISubscriber<CursorScroll>
    {
        private readonly Camera2D _camera;
        private readonly IScreen _screen;

        public CameraEngine(IScreen screen, Camera2D camera)
        {
            _screen = screen;
            _camera = camera;
            _camera.Zoom = 100;
        }

        public string name { get; } = nameof(CameraEngine);

        public void Step(in GameTime _param)
        {
            _screen.Camera.Update(_param);
            _camera.Update(_param);

            if(Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                _camera.TargetPosition -= Vector2.UnitY * (float)_param.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                _camera.TargetPosition += Vector2.UnitY * (float)_param.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _camera.TargetPosition -= Vector2.UnitX * (float)_param.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _camera.TargetPosition += Vector2.UnitX * (float)_param.ElapsedGameTime.TotalSeconds;
            }
        }

        public void Process(in Guid messageId, in CursorScroll message)
        {
            _camera.TargetZoom *= ((float)Math.Pow(1.5, message.Delta / 120));
        }
    }
}
