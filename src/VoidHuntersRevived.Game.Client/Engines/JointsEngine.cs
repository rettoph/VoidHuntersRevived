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

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [Sequence<DrawEngineSequence>(DrawEngineSequence.PostDraw)]
    [SimulationTypeFilter(SimulationType.Predictive)]
    internal class JointsEngine : BasicEngine, IStepEngine<GameTime>
    {
        private readonly IScreen _screen;
        private readonly Camera2D _camera;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly PrimitiveShape _jointShape;

        public JointsEngine(IScreen screen, Camera2D camera, PrimitiveBatch<VertexPositionColor> primitiveBatch)
        {
            _screen = screen;
            _camera = camera;
            _primitiveBatch = primitiveBatch;
            _jointShape = new ProjectedShape(camera, new[]
            {
                new Vector2(-0.1f, -0.15f),
                new Vector2(0f, 0f),
                new Vector2(-0.1f, 0.15f),
            });
        }

        public string name { get; } = nameof(JointsEngine);

        public void Step(in GameTime _param)
        {
            _primitiveBatch.Begin(_screen.Camera);
            var groups = this.entitiesDB.FindGroups<Joints, Node>();
            foreach (var ((joints, nodes, count), _) in this.entitiesDB.QueryEntities<Joints, Node>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    for(int j=0; j < joints[i].Parents.count; j++)
                    {
                        FixMatrix jointTransformation = joints[i].Parents[j].Location.Transformation * nodes[i].Transformation;
                        _primitiveBatch.Trace(_jointShape, Color.Red, jointTransformation.XnaMatrix);
                    }
                    
                }
            }
            _primitiveBatch.End();
        }
    }
}
