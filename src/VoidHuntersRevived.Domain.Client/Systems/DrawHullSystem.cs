using Guppy.MonoGame;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal sealed partial class DrawHullSystem : EntityDrawSystem
    {
        private static readonly AspectBuilder HullAspect = Aspect.All(new[]
        {
            typeof(Hull),
            typeof(Predictive),
            typeof(AetherLeaf)
        });

        private readonly IShipPartConfigurationService _configurations;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly Camera2D _camera;
        private readonly Dictionary<Hull, HullRenderer> _renderers;

        private ComponentMapper<Hull> _hulls;
        private ComponentMapper<AetherLeaf> _leaves;
        private ComponentMapper<Body> _bodies;

        public DrawHullSystem(
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            Camera2D camera,
            IShipPartConfigurationService configurations) : base(HullAspect)
        {
            _primitiveBatch = primitiveBatch;
            _camera = camera;
            _configurations = configurations;
            _renderers = new Dictionary<Hull, HullRenderer>();

            _hulls = default!;
            _leaves = default!;
            _bodies = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _hulls = mapperService.GetMapper<Hull>();
            _leaves = mapperService.GetMapper<AetherLeaf>();
            _bodies = mapperService.GetMapper<Body>();

            foreach (var hull in _configurations.GetAll<Hull>())
            {
                _renderers.Add(hull, new HullRenderer(_primitiveBatch, hull));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _primitiveBatch.Begin(_camera);

            foreach(var entityId in this.subscription.ActiveEntities)
            {
                var hull = _hulls.Get(entityId);
                var leaf = _leaves.Get(entityId);
                var body = _bodies.Get(leaf.Tree.Entity.Id);

                var transformation = Matrix.CreateTranslation(body.Position.X, body.Position.Y, 0);
                _renderers[hull].Render(transformation);
            }

            _primitiveBatch.End();
        }
    }
}
