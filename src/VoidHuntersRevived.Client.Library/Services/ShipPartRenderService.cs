using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.System.Collections;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Guppy.Utilities.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Extensions.Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Client.Library.Services
{
    /// <summary>
    /// Simple service useful for rendering a ship part
    /// directly to screen with little passing from
    /// the ship part itself.
    /// </summary>
    public class ShipPartRenderService : Frameable
    {
        #region Constants
        private static Color TransparentWhite = new Color(255, 255, 255, 0);
        #endregion

        #region Private Structs
        private struct ShipPartContextPrimitiveData
        {
            public PrimitivePath[] OuterHulls;
            public PrimitiveShape[] InnerShapes;
            public PrimitivePath MaleNode;
        }
        #endregion

        #region Private Fields
        private Dictionary<ShipPartContext, ShipPartContextPrimitiveData> _primitives;

        private Single _configuredZoom;
        private Camera2D _camera;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private Single _width = 1;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            _primitives = new Dictionary<ShipPartContext, ShipPartContextPrimitiveData>();

            provider.Service(out _camera);
            provider.Service(out _primitiveBatch);
        }

        protected override void Release()
        {
            base.Release();

            _camera = null;
            _primitiveBatch = null;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(_camera.Zoom != _configuredZoom && Math.Abs(_configuredZoom - _camera.Zoom) / _configuredZoom > 0.005f)
            {
                _configuredZoom = _camera.Zoom;
                _width = (1 / _camera.Zoom);

                foreach(ShipPartContextPrimitiveData primitives in _primitives.Values)
                {
                    primitives.OuterHulls.ForEach(hull => hull.Width = _width);
                    primitives.MaleNode.Width = _width;
                }
            }
        }

        /// <summary>
        /// Render the specific ShipPart recieved, if possible.
        /// </summary>
        /// <param name="shipPart"></param>
        public void Render(ShipPart shipPart)
        {
            _primitives[shipPart.Context].InnerShapes.ForEach(shape =>
            { // Draw all inner shapes...
                _primitiveBatch.DrawPrimitive(
                    shape,
                    Color.Lerp(shipPart.Color, Color.Transparent, 0.25f),
                    shipPart.WorldTransformation);
            });

            _primitives[shipPart.Context].OuterHulls.ForEach(hull =>
            { // Draw all outer hulls...
                _primitiveBatch.DrawPrimitive(
                    hull,
                    Color.Lerp(shipPart.Color, ShipPartRenderService.TransparentWhite, 0.25f),
                    shipPart.WorldTransformation);
            });

            // Draw part paths...
            _primitiveBatch.DrawPrimitive(
                _primitives[shipPart.Context].MaleNode,
                Color.Lerp(shipPart.Color, ShipPartRenderService.TransparentWhite, 0.5f),
                shipPart.WorldTransformation);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Create a new <see cref="ShipPartContextPrimitiveData"/> instance
        /// assuming that the recieved <see cref="ShipPart.Context"/>
        /// </summary>
        /// <param name="shipPart"></param>
        public void ValidateContext(ShipPart shipPart)
        {
            if (_primitives.ContainsKey(shipPart.Context))
                return;

            _primitives[shipPart.Context] = new ShipPartContextPrimitiveData()
            {
                OuterHulls = shipPart.Context.OuterHulls.Select(h => PrimitivePath.Create(_width, h)).ToArray(),
                InnerShapes = shipPart.Context.InnerShapes.Select(s => PrimitiveShape.Create(s)).ToArray(),
                MaleNode = PrimitivePath.Create(
                    _width,
                    shipPart.MaleConnectionNode.LocalPosition + (Vector2.UnitX * 0.2f).RotateTo(shipPart.MaleConnectionNode.LocalRotation + MathHelper.Pi + MathHelper.PiOver4),
                    shipPart.MaleConnectionNode.LocalPosition,
                    shipPart.MaleConnectionNode.LocalPosition + (Vector2.UnitX * 0.2f).RotateTo(shipPart.MaleConnectionNode.LocalRotation + MathHelper.Pi - MathHelper.PiOver4))
            };
        }


        /// <summary>
        /// Remove the specified <paramref name="shipPart"/>s Context
        /// from the internal <see cref="ShipPartContextPrimitiveData"/>
        /// cache. Generally this never needs to be called.
        /// </summary>
        /// <param name="shipPart"></param>
        public void RemoveContext(ShipPart shipPart)
        {
            if(shipPart != default && _primitives.ContainsKey(shipPart.Context))
                _primitives.Remove(shipPart.Context);
        }
        #endregion
    }
}
