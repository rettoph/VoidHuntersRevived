using Guppy.MonoGame;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
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
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Configurations;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal sealed partial class DrawSystem : EntityDrawSystem
    {
        private static readonly AspectBuilder HullAspect = Aspect.All(new[]
        {
            typeof(Rigid),
            typeof(Predictive),
            typeof(AetherLeaf)
        });

        private readonly IShipPartConfigurationService _configurations;
        private readonly IResourceProvider _resources;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly Camera2D _camera;
        private readonly Dictionary<DrawConfiguration, Renderer> _renderers;

        private ComponentMapper<Draw> _draws;
        private ComponentMapper<AetherLeaf> _leaves;
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<Linked> _linked;

        public DrawSystem(
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            Camera2D camera,
            IResourceProvider resources,
            IShipPartConfigurationService configurations) : base(HullAspect)
        {
            _primitiveBatch = primitiveBatch;
            _camera = camera;
            _configurations = configurations;
            _resources = resources;
            _renderers = new Dictionary<DrawConfiguration, Renderer>();

            _draws = default!;
            _leaves = default!;
            _bodies = default!;
            _linked = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _draws = mapperService.GetMapper<Draw>();
            _leaves = mapperService.GetMapper<AetherLeaf>();
            _bodies = mapperService.GetMapper<Body>();
            _linked = mapperService.GetMapper<Linked>();

            foreach (var configuration in _configurations.GetAll<DrawConfiguration>())
            {
                _renderers.Add(configuration, new Renderer(_primitiveBatch, _resources, configuration));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _primitiveBatch.Begin(_camera);

            foreach(var entityId in this.subscription.ActiveEntities)
            {
                var draw = _draws.Get(entityId);
                var leaf = _leaves.Get(entityId);
                var linked = _linked.Get(entityId);
                var body = _bodies.Get(leaf.Tree.Entity.Id);

                var transformation = linked is null ? Matrix.Identity : linked.Transformation;
                transformation *= Matrix.CreateTranslation(body.Position.X, body.Position.Y, 0);

                _renderers[draw.Configuration].Render(transformation);
            }

            _primitiveBatch.End();
        }
    }
}
