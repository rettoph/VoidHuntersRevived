using Guppy.Attributes;
using Guppy.Common;
using Guppy.Loaders;
using Guppy.MonoGame;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Diagnostics;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Games;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Simulations;
using VoidHuntersRevived.Library.Simulations.EventTypes;
using VoidHuntersRevived.Library.Simulations.Systems;

namespace VoidHuntersRevived.Client.Library.Systems
{
    internal sealed class AetherDebugSystem : DrawSystem, ILockstepSimulationSystem, 
        ISubscriber<Tick>, 
        ISubscriber<BodyPosition>
    {
        private readonly ISimulationService _simulations;
        private DebugView _debugLockstep;
        private DebugView _debugPredictive;
        private readonly GraphicsDevice _graphics;
        private readonly ContentManager _content;
        private readonly Camera2D _camera;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly Lazy<AetherBodyPositionDebugService> _bodyPositionDebugService;

        public AetherDebugSystem(
            Camera2D camera,
            ISimulationService simulations, 
            GraphicsDevice graphics, 
            ContentManager content,
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            Lazy<AetherBodyPositionDebugService> bodyPositionDebugService)
        {
            _camera = camera;
            _simulations = simulations;
            _graphics = graphics;
            _content = content;
            _primitiveBatch = primitiveBatch;
            _bodyPositionDebugService = bodyPositionDebugService;
            _debugLockstep = default!;
            _debugPredictive = default!;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _debugLockstep = new DebugView(_simulations[SimulationType.Lockstep].Aether);
            _debugLockstep.LoadContent(_graphics, _content);

            _debugPredictive = new DebugView(_simulations[SimulationType.Predictive].Aether);
            _debugPredictive.LoadContent(_graphics, _content);
        }

        public override void Draw(GameTime gameTime)
        {
            // _debugLockstep.RenderDebugData(_camera.Projection, _camera.View);
            _debugPredictive.RenderDebugData(_camera.Projection, _camera.View);

            _primitiveBatch.Begin(_camera);
            _bodyPositionDebugService.Value.Draw(_primitiveBatch);
            _primitiveBatch.End();
        }

        public void Process(in Tick message)
        {
            foreach(var body in _simulations[SimulationType.Predictive].Aether.BodyList)
            {
                _bodyPositionDebugService.Value.AddPosition(true, body.GetHashCode(), body.Position);
            }
        }

        public void Process(in BodyPosition message)
        {
            _bodyPositionDebugService.Value.AddPosition(false, message.Id, message.Position);
        }
    }
}
