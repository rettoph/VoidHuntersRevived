using Guppy.MonoGame;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Simulations.Components;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal sealed partial class DrawableSystem : EntityDrawSystem
    {
        private static readonly AspectBuilder HullAspect = Aspect.All(new[]
        {
            typeof(Drawable),
            typeof(Predictive),
            typeof(Node)
        });

        private readonly IShipPartConfigurationService _configurations;
        private readonly IResourceProvider _resources;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly Camera2D _camera;
        private readonly Dictionary<DrawConfiguration, Renderer> _renderers;

        private ComponentMapper<Drawable> _drawables;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<Jointed> _jointed;

        public DrawableSystem(
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

            _drawables = default!;
            _nodes = default!;
            _bodies = default!;
            _jointed = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _drawables = mapperService.GetMapper<Drawable>();
            _nodes = mapperService.GetMapper<Node>();
            _bodies = mapperService.GetMapper<Body>();
            _jointed = mapperService.GetMapper<Jointed>();

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
                var drawable = _drawables.Get(entityId);
                var node = _nodes.Get(entityId);

                _renderers[drawable.Configuration].Render(node.WorldTransformation);
            }

            _primitiveBatch.End();
        }
    }
}
