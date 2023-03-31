using Guppy.Attributes;
using Guppy.Common;
using Guppy.GUI;
using Guppy.MonoGame;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Simulations.Components;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    [GuppyFilter<LocalGameGuppy>]
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
        private readonly IScreen _screen;
        private readonly Dictionary<DrawConfiguration, Renderer> _renderers;

        private ComponentMapper<Drawable> _drawables;
        private ComponentMapper<Node> _nodes;

        public DrawableSystem(
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            Camera2D camera,
            IScreen screen,
            IResourceProvider resources,
            IShipPartConfigurationService configurations) : base(HullAspect)
        {
            _primitiveBatch = primitiveBatch;
            _camera = camera;
            _screen = screen;
            _configurations = configurations;
            _resources = resources;
            _renderers = new Dictionary<DrawConfiguration, Renderer>();

            _drawables = default!;
            _nodes = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _drawables = mapperService.GetMapper<Drawable>();
            _nodes = mapperService.GetMapper<Node>();

            foreach (var configuration in _configurations.GetAll<DrawConfiguration>())
            {
                _renderers.Add(configuration, new Renderer(_camera, _primitiveBatch, _resources, configuration));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _primitiveBatch.Begin(_camera);

            foreach(var entityId in this.subscription.ActiveEntities)
            {
                var drawable = _drawables.Get(entityId);
                var node = _nodes.Get(entityId);

                _renderers[drawable.Configuration].RenderShapes(node.WorldTransformation);
            }

            _primitiveBatch.End();

            _primitiveBatch.Begin(_screen.Camera);

            foreach (var entityId in this.subscription.ActiveEntities)
            {
                var drawable = _drawables.Get(entityId);
                var node = _nodes.Get(entityId);

                _renderers[drawable.Configuration].RenderPaths(node.WorldTransformation);
            }

            _primitiveBatch.End();
        }
    }
}
