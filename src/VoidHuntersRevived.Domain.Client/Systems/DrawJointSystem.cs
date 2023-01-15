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
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Configurations;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Domain.Entities.Components;
using MonoGame.Extended.Entities.Systems;
using Guppy.Common.Collections;
using tainicom.Aether.Physics2D.Common;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal sealed class DrawJointSystem : EntityDrawSystem
    {
        private static readonly AspectBuilder LinkableAspect = Aspect.All(new[]
        {
            typeof(Linkable)
        });

        private readonly IShipPartConfigurationService _configurations;
        private readonly IResourceProvider _resources;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly Camera2D _camera;
        private Vector2[] _buffer;
        private Vector2[] _vertices;

        private ComponentMapper<Linkable> _linkables;

        public DrawJointSystem(
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            Camera2D camera,
            IResourceProvider resources,
            IShipPartConfigurationService configurations) : base(LinkableAspect)
        {
            _primitiveBatch = primitiveBatch;
            _camera = camera;
            _configurations = configurations;
            _resources = resources;
            _buffer = new Vector2[2];
            _vertices = new[] 
            {
                new Vector2(-0.25f, 0),
                new Vector2(0.25f, 0)
            };


            _linkables = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _linkables = mapperService.GetMapper<Linkable>();
        }

        public override void Draw(GameTime gameTime)
        {
            _primitiveBatch.Begin(_camera);

            foreach (var entityId in this.subscription.ActiveEntities)
            {
                var linkable = _linkables.Get(entityId);

                this.DrawJoints(linkable.Joints);
            }

            _primitiveBatch.End();
        }

        private void DrawJoints(Linkable.Joint[] joints)
        {
            foreach(var joint in joints)
            {
                 Vector2.Transform(ref _vertices[0], ref joint.Transformation, out _buffer[0]);
                 Vector2.Transform(ref _vertices[1], ref joint.Transformation, out _buffer[1]);
                
                _primitiveBatch.DrawLine(Color.Green, _buffer[0], _buffer[1]);
            }
        }
    }
}
