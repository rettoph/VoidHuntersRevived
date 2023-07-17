using Guppy.Attributes;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Resources;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    internal class JointsEngine : BasicEngine, IStepEngine<GameTime>
    {
        private readonly Camera2D _camera;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly PrimitiveShape _jointShape;

        public JointsEngine(Camera2D camera, PrimitiveBatch<VertexPositionColor> primitiveBatch)
        {
            _camera = camera;
            _primitiveBatch = primitiveBatch;
            _jointShape = new ProjectedShape(camera, new[]
            {
                Vector2.Zero,
                Vector2.UnitX,
            });
        }

        public string name { get; } = nameof(JointsEngine);

        public void Step(in GameTime _param)
        {
            _primitiveBatch.Begin(_camera);
            var groups = this.entitiesDB.FindGroups<Joints, Node>();
            foreach (var ((joints, nodes, count), _) in this.entitiesDB.QueryEntities<Joints, Node>(groups))
            {
                for (int i = 0; i < count; i++)
                {

                }
            }
            _primitiveBatch.End();
        }
    }
}
