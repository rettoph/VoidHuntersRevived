using Guppy.GUI;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Simulations.Components;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal sealed partial class DrawableShipPartSystem : EntityDrawSystem
    {
        private static readonly AspectBuilder ShipPartAspect = Aspect.All(new[]
        {
            typeof(Predictive),
            typeof(ShipPart),
            typeof(Drawable),
            typeof(WorldLocation)
        });

        private readonly IResourceProvider _resources;
        private readonly IScreen _screen;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly Camera2D _camera;
        private readonly Dictionary<Drawable, Renderer> _renderers;
        private ComponentMapper<ShipPart> _shipPart = null!;
        private ComponentMapper<WorldLocation> _worldLocations = null!;
        private ComponentMapper<Drawable> _drawable = null!;

        public DrawableShipPartSystem(
            IResourceProvider resources,
            IScreen screen,
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            Camera2D camera) : base(ShipPartAspect)
        {
            _resources = resources;
            _screen = screen;
            _primitiveBatch = primitiveBatch;
            _camera = camera;
            _renderers = new Dictionary<Drawable, Renderer>();
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _shipPart = mapperService.GetMapper<ShipPart>();
            _worldLocations = mapperService.GetMapper<WorldLocation>();
            _drawable = mapperService.GetMapper<Drawable>();
        }

        public override void Draw(GameTime gameTime)
        {
            _primitiveBatch.Begin(_camera);
            foreach(int entityId in this.ActiveEntities)
            {
                WorldLocation worldLocation = _worldLocations.Get(entityId);
                Drawable drawable = _drawable.Get(entityId);
            
                ref Renderer? renderer = ref CollectionsMarshal.GetValueRefOrAddDefault(_renderers, drawable, out bool exists);
            
                if(!exists)
                {
                    renderer = new Renderer(_camera, _primitiveBatch, _resources, drawable);
                }
            
                renderer!.RenderShapes(worldLocation.Transformation);
            }
            _primitiveBatch.End();

            _primitiveBatch.Begin(_screen.Camera);
            foreach (int entityId in this.ActiveEntities)
            {
                WorldLocation worldLocation = _worldLocations.Get(entityId);
                Drawable drawable = _drawable.Get(entityId);
                Renderer renderer = _renderers[drawable];

                renderer!.RenderPaths(worldLocation.Transformation);
            }
            _primitiveBatch.End();
        }
    }
}
