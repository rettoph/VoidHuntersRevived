using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.Input;
using Guppy.Game.Input.Messages;
using Guppy.Game.MonoGame;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Svelto.ECS;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [SimulationFilter(SimulationType.Lockstep)]
    [Sequence<DrawSequence>(DrawSequence.PreDraw)]
    internal sealed class CameraEngine : BasicEngine, IStepEngine<GameTime>,
        IInputSubscriber<CursorScroll>
    {
        private readonly Camera2D _camera;
        private readonly IScreen _screen;
        private Vector2 _offset;

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

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                _offset -= Vector2.UnitY * (float)_param.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                _offset += Vector2.UnitY * (float)_param.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _offset -= Vector2.UnitX * (float)_param.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _offset += Vector2.UnitX * (float)_param.ElapsedGameTime.TotalSeconds;
            }

            _camera.TargetPosition = _offset;
        }

        public void Process(in Guid messageId, CursorScroll message)
        {
            _camera.TargetZoom *= ((float)Math.Pow(1.5, message.Delta / 120));
        }
    }
}
