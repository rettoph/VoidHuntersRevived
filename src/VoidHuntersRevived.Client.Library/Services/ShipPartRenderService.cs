using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.System.Collections;
using Guppy.Extensions.Utilities;
using Guppy.Services;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Guppy.Utilities.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using VoidHuntersRevived.Library.Dtos.Utilities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Services
{
    public class ShipPartRenderService : Frameable
    {
        #region Constants
        private static Color TransparentWhite = new Color(255, 255, 255, 0);
        #endregion

        #region Private Structs
        private class ShipPartContextPrimitiveData
        {
            public Color DefaultShapeColor;
            public Color DefaultPathColor;
            public Boolean InheritColor;
            
            public PrimitiveShape[] Shapes;
            public PrimitivePath[] Paths;
        }
        #endregion

        #region Private Fields
        private Dictionary<UInt32, ShipPartContextPrimitiveData> _primitives;

        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private ShipPartService _shipParts;
        private ColorService _colors;

        private PrimitivePath _estrangedNodePrimitive;
        private PrimitivePath _nodePrimitive;

        private Camera2D _camera;

        private Single _configuredZoom;
        private Single _width = 1;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            _primitives = new Dictionary<UInt32, ShipPartContextPrimitiveData>();

            provider.Service(out _primitiveBatch);
            provider.Service(out _shipParts);
            provider.Service(out _colors);
            provider.Service(out _camera);

            _estrangedNodePrimitive = PrimitivePath.Create(0.025f, new Vector2(0.1f, -0.075f), new Vector2(0, 0), new Vector2(0.1f, 0.075f));
            _nodePrimitive = PrimitivePath.Create(0.025f, new Vector2(0.075f, 0), new Vector2(0, 0));
        }

        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            foreach (ShipPartContext context in _shipParts.RegisteredContexts.Values)
                this.RegisterContextPrimitiveData(context);

            _shipParts.OnContextRegistered += this.HandleContextRegistered;
        }

        protected override void Release()
        {
            base.Release();

            _shipParts.OnContextRegistered -= this.HandleContextRegistered;
        }

        protected override void PostRelease()
        {
            base.Release();

            _primitives.Clear();

            _primitiveBatch = null;
            _shipParts = null;
            _colors = null;
            _camera = null;
        }
        #endregion

        #region Frame Methods
        /// <summary>
        /// Render the specific ShipPart recieved, if possible.
        /// </summary>
        /// <param name="shipPart"></param>
        public void Render(ShipPart shipPart, ref Matrix worldTransformation)
        {
            ShipPartContextPrimitiveData primitiveData = _primitives[shipPart.Context.Id];

            Color shapeColor = primitiveData.DefaultShapeColor;
            Color pathColor = primitiveData.DefaultPathColor;

            if(primitiveData.InheritColor && shipPart.Chain.Color.HasValue)
            {
                shapeColor = Color.Lerp(shipPart.Chain.Color.Value, Color.Transparent, 0.25f);
                pathColor = Color.Lerp(shipPart.Chain.Color.Value, ShipPartRenderService.TransparentWhite, 0.25f);
            }


            foreach (PrimitiveShape shape in primitiveData.Shapes)
            { // Draw all shapes...
                _primitiveBatch.DrawPrimitive(
                    shape,
                    shapeColor,
                    worldTransformation);
            }

            foreach (PrimitivePath path in primitiveData.Paths)
            { // Draw all paths...
                _primitiveBatch.DrawPrimitive(
                    path,
                    pathColor,
                    worldTransformation);
            }

            foreach (ConnectionNode node in shipPart.ConnectionNodes)
            {
                var nodeWorldTransformation = node.LocalTransformationMatrix * worldTransformation;
            
                if(node.Connection.State == ConnectionNodeState.Estranged)
                {
                    _primitiveBatch.DrawPrimitive(
                        _estrangedNodePrimitive,
                        pathColor,
                        nodeWorldTransformation);
                }
                else
                {
                    _primitiveBatch.DrawPrimitive(
                        _nodePrimitive,
                        pathColor,
                        nodeWorldTransformation);
                }
            }
        }

        public void Clean()
        {
            if (_camera.Zoom != _configuredZoom && Math.Abs(_configuredZoom - _camera.Zoom) / _configuredZoom > 0.005f)
            {
                _configuredZoom = _camera.Zoom;
                _width = (1 / _camera.Zoom);

                foreach (ShipPartContextPrimitiveData primitives in _primitives.Values)
                {
                    foreach(PrimitivePath path in primitives.Paths)
                    {
                        path.Width = _width;
                    }
                }

                _nodePrimitive.Width = _width;
                _estrangedNodePrimitive.Width = _width;
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Construct a new <see cref="ShipPartContextPrimitiveData"/> instance based on
        /// ths given <paramref name="context"/> value.
        /// </summary>
        /// <param name="context"></param>
        private void RegisterContextPrimitiveData(ShipPartContext context)
        {
            _primitives[context.Id] = new ShipPartContextPrimitiveData()
            {
                DefaultShapeColor = Color.Lerp(_colors[context.Color], Color.Transparent, 0.25f),
                DefaultPathColor = Color.Lerp(_colors[context.Color], ShipPartRenderService.TransparentWhite, 0.25f),
                InheritColor = context.InheritColor,
                Shapes = context.Shapes.Where(s => s.IsVisible).Select(s =>
                {
                    return s.Data switch
                    {
                        PolygonShape polygon => PrimitiveShape.Create(polygon.Vertices),
                        _ => throw new ArgumentOutOfRangeException($"Unable to create PrimitiveShape for {s.Data.ShapeType}")
                    };
                }).ToArray(),
                Paths = context.Paths.Select(p =>
                {
                    return PrimitivePath.Create(0.25f, p);
                }).ToArray()
            };
        }
        #endregion

        #region Event Handlers
        private void HandleContextRegistered(ShipPartService sender, ShipPartContext args)
            => this.RegisterContextPrimitiveData(args);
        #endregion
    }
}
