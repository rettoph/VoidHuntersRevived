using Guppy.Attributes;
using Guppy.Common;
using Guppy.GUI;
using Guppy.Input.Messages;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
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
        }

        public void Process(in Guid messageId, in CursorScroll message)
        {
            _camera.TargetZoom *= ((float)Math.Pow(1.5, message.Delta / 120));
        }
    }
}
