using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.GUI;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Enums;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [Sequence<DrawEngineSequence>(DrawEngineSequence.PostDraw)]
    [SimulationTypeFilter(SimulationType.Predictive)]
    internal class SocketsEngine : BasicEngine, IStepEngine<GameTime>
    {
        private readonly IScreen _screen;
        private readonly IEntityService _entities;
        private readonly Camera2D _camera;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly PrimitiveShape _jointShape;

        public SocketsEngine(IScreen screen, IEntityService entities, Camera2D camera, PrimitiveBatch<VertexPositionColor> primitiveBatch)
        {
            _screen = screen;
            _entities = entities;
            _camera = camera;
            _primitiveBatch = primitiveBatch;
            _jointShape = new ProjectedShape(camera, new[]
            {
                new Vector2(-0.1f, -0.15f),
                new Vector2(0f, 0f),
                new Vector2(-0.1f, 0.15f),
            });
        }

        public string name { get; } = nameof(SocketsEngine);

        public void Step(in GameTime _param)
        {
            _primitiveBatch.Begin(_screen.Camera);
            foreach (var ((statuses, joints, nodes, count), _) in _entities.QueryEntities<EntityStatus, Sockets, Node>())
            {
                for (int i = 0; i < count; i++)
                {
                    for(int j=0; j < joints[i].Items.count; j++)
                    {
                        if (statuses[i].IsSpawned)
                        {
                            FixMatrix jointTransformation = joints[i].Items[j].Location.Transformation * nodes[i].Transformation;
                            _primitiveBatch.Trace(_jointShape, Color.Red, jointTransformation.XnaMatrix);
                        }
                    }
                    
                }
            }
            _primitiveBatch.End();
        }
    }
}
