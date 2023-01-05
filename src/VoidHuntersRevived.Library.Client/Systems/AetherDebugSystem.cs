using Guppy.MonoGame.Utilities.Cameras;
using Guppy.MonoGame;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using MonoGame.Extended.Entities.Systems;
using tainicom.Aether.Physics2D.Diagnostics;
using MonoGame.Extended.Entities;
using Guppy.Network;
using Guppy.Network.Enums;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Library.Client.Systems
{
    internal sealed class AetherDebugSystem : DrawSystem
    {
        private readonly ISimulationService _simulations;
        private DebugView[] _debugViews;
        private readonly GraphicsDevice _graphics;
        private readonly ContentManager _content;
        private readonly Camera2D _camera;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly NetScope _netScope;

        public AetherDebugSystem(
            Camera2D camera,
            ISimulationService simulations,
            GraphicsDevice graphics,
            ContentManager content,
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            NetScope netScope)
        {
            _camera = camera;
            _simulations = simulations;
            _graphics = graphics;
            _content = content;
            _primitiveBatch = primitiveBatch;
            _debugViews = new DebugView[_simulations.Instances.Count()];
            _netScope = netScope;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            for(var i=0; i < _simulations.Instances.Count; i++)
            {
                var color = 
                _debugViews[i] = new DebugView(_simulations.Instances[i].Aether)
                {
                    DefaultShapeColor = (_simulations.Instances[i].Type, _netScope.Peer?.Type) switch
                    {
                        (SimulationType.Lockstep, PeerType.Server) => Color.Green,
                        (SimulationType.Lockstep, PeerType.Client) => Color.Yellow,
                        (SimulationType.Predictive, PeerType.Client) => Color.Pink,
                        _ => Color.White
                    }
                };
                _debugViews[i].LoadContent(_graphics, _content);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach(DebugView view in _debugViews)
            {
                view.RenderDebugData(_camera.Projection, _camera.View);
            }
        }
    }
}
