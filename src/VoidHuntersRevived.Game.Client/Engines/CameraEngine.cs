using Guppy.Attributes;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
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
    internal sealed class CameraEngine : BasicEngine, IStepEngine<GameTime>
    {
        private readonly Camera2D _camera;

        public CameraEngine(Camera2D camera)
        {
            _camera = camera;
            _camera.Zoom = 100;
        }

        public string name { get; } = nameof(CameraEngine);

        public void Step(in GameTime _param)
        {
            _camera.Update(_param);
        }
    }
}
