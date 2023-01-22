using Guppy.MonoGame.Utilities.Cameras;
using Guppy.MonoGame;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Simulations.Components;
using MonoGame.Extended.Entities.Systems;
using Guppy.Common.Collections;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using Guppy.MonoGame.Primitives;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal sealed class DrawJointSystem : EntityDrawSystem
    {
        private static readonly AspectBuilder LinkableAspect = Aspect.All(new[]
        {
            typeof(Jointable),
            typeof(Node),
            typeof(Predictive)
        });

        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly Camera2D _camera;
        private PrimitiveShape _shape;

        private ComponentMapper<Body> _bodies;
        private ComponentMapper<Jointable> _jointables;
        private ComponentMapper<Node> _nodes;

        public DrawJointSystem(
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            Camera2D camera) : base(LinkableAspect)
        {
            _primitiveBatch = primitiveBatch;
            _camera = camera;
            _shape = new PrimitiveShape(new[] 
            {
                new Vector2(-0.25f, 0),
                new Vector2(0.25f, 0)
            });


            _jointables = default!;
            _nodes = default!;
            _bodies = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _jointables = mapperService.GetMapper<Jointable>();
            _nodes = mapperService.GetMapper<Node>();
            _bodies = mapperService.GetMapper<Body>();
        }

        public override void Draw(GameTime gameTime)
        {
            _primitiveBatch.Begin(_camera);

            foreach (var entityId in this.subscription.ActiveEntities)
            {
                var jointable = _jointables.Get(entityId);
                var joint = _nodes.Get(entityId);
                var body = _bodies.Get(joint.Tree);

                this.DrawJoints(jointable.Joints, body.Position, body.Rotation);
            }

            _primitiveBatch.End();
        }

        private void DrawJoints(Jointable.Joint[] joints, Vector2 position, float rotation)
        {
            var world = Matrix.CreateRotationZ(rotation);
            world *= Matrix.CreateTranslation(position.X, position.Y, 0);

            foreach (var joint in joints)
            {
                var transformation = joint.LocalTransformation * world;
                var color = Color.Red;

                _primitiveBatch.Trace(_shape, in color, ref transformation);
            }
        }
    }
}
