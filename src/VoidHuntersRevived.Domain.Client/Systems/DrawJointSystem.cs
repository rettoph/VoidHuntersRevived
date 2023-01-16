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
using VoidHuntersRevived.Domain.Entities.Components;
using MonoGame.Extended.Entities.Systems;
using Guppy.Common.Collections;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

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
        private Vector2[] _buffer;
        private Vector2[] _vertices;

        private ComponentMapper<Body> _bodies;
        private ComponentMapper<Jointable> _linkables;
        private ComponentMapper<Node> _leaves;

        public DrawJointSystem(
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            Camera2D camera) : base(LinkableAspect)
        {
            _primitiveBatch = primitiveBatch;
            _camera = camera;
            _buffer = new Vector2[2];
            _vertices = new[] 
            {
                new Vector2(-0.25f, 0),
                new Vector2(0.25f, 0)
            };


            _linkables = default!;
            _leaves = default!;
            _bodies = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _linkables = mapperService.GetMapper<Jointable>();
            _leaves = mapperService.GetMapper<Node>();
            _bodies = mapperService.GetMapper<Body>();
        }

        public override void Draw(GameTime gameTime)
        {
            _primitiveBatch.Begin(_camera);

            foreach (var entityId in this.subscription.ActiveEntities)
            {
                var linkable = _linkables.Get(entityId);
                var leaf = _leaves.Get(entityId);
                var body = _bodies.Get(leaf.Tree.Entity.Id);

                this.DrawJoints(linkable.Joints, body.Position, body.Rotation);
            }

            _primitiveBatch.End();
        }

        private void DrawJoints(Jointable.Joint[] joints, Vector2 position, float rotation)
        {
            var world = Matrix.CreateRotationZ(rotation);
            world *= Matrix.CreateTranslation(position.X, position.Y, 0);
            int i = 0;

            foreach (var joint in joints)
            {
                var transformation = joint.LocalTransformation * world;

                Vector2.Transform(ref _vertices[0], ref transformation, out _buffer[0]);
                Vector2.Transform(ref _vertices[1], ref transformation, out _buffer[1]);
                
                _primitiveBatch.DrawLine(i++ == 0 ? Color.Green : Color.Red, _buffer[0], _buffer[1]);
            }
        }
    }
}
