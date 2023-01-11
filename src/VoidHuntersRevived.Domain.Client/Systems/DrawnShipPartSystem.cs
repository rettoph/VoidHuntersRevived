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
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal sealed partial class DrawnShipPartSystem : EntityDrawSystem
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
        private readonly Dictionary<Drawn, Renderer> _renderers;

        private ComponentMapper<Drawn> _drawn;
        private ComponentMapper<AetherLeaf> _leaves;
        private ComponentMapper<Body> _bodies;

        public DrawnShipPartSystem(
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            Camera2D camera,
            IResourceProvider resources,
            IShipPartConfigurationService configurations) : base(HullAspect)
        {
            _primitiveBatch = primitiveBatch;
            _camera = camera;
            _configurations = configurations;
            _resources = resources;
            _renderers = new Dictionary<Drawn, Renderer>();

            _drawn = default!;
            _leaves = default!;
            _bodies = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _drawn = mapperService.GetMapper<Drawn>();
            _leaves = mapperService.GetMapper<AetherLeaf>();
            _bodies = mapperService.GetMapper<Body>();

            foreach (var drawn in _configurations.GetAll<Drawn>())
            {
                _renderers.Add(drawn, new Renderer(_primitiveBatch, _resources, drawn));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _primitiveBatch.Begin(_camera);

            foreach(var entityId in this.subscription.ActiveEntities)
            {
                var hull = _drawn.Get(entityId);
                var leaf = _leaves.Get(entityId);
                var body = _bodies.Get(leaf.Tree.Entity.Id);

                var transformation = Matrix.CreateTranslation(body.Position.X, body.Position.Y, 0);
                _renderers[hull].Render(transformation);
            }

            _primitiveBatch.End();
        }
    }
}
