using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Diagnostics;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Client.Debuggers;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal sealed class DebugAetherSystem : DrawSystem
    {
        private readonly ISimulationService _simulations;
        private readonly DebugView _view;
        private readonly Camera2D _camera;

        public DebugAetherSystem(
            ISimulationService simulations,
            GraphicsDevice graphics,
            ContentManager content,
            Camera2D camera)
        {
            _simulations = simulations;
            _camera = camera;
            _view = new DebugView(_simulations.First(SimulationType.Predictive, SimulationType.Lockstep).Aether);

            _view.LoadContent(graphics, content);
        }

        public override void Draw(GameTime gameTime)
        {
            _view.RenderDebugData(_camera.Projection, _camera.View);
        }
    }
}
