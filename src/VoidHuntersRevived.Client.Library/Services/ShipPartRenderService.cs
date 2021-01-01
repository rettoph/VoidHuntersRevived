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
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;

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
        private struct ShipPartConfigurationPrimitiveData
        {
            public PrimitivePath Path;
            public PrimitiveShape[] Shapes;
        }
        #endregion

        #region Private Fields
        private Dictionary<ShipPartConfiguration, ShipPartConfigurationPrimitiveData> _primitives;

        private Single _configuredZoom;
        private Camera2D _camera;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private Single _width = 1;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            _primitives = new Dictionary<ShipPartConfiguration, ShipPartConfigurationPrimitiveData>();

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

                foreach(ShipPartConfigurationPrimitiveData primitives in _primitives.Values)
                    primitives.Path.Width = _width;
            }
        }

        /// <summary>
        /// Render the specific ShipPart recieved, if possible.
        /// </summary>
        /// <param name="shipPart"></param>
        public void Render(ShipPart shipPart)
        {
            _primitives[shipPart.Configuration].Shapes.ForEach(shape =>
            { // Draw all part shapes...
                _primitiveBatch.DrawPrimitive(
                    shape,
                    Color.Lerp(shipPart.Color, Color.Transparent, 0.25f),
                    shipPart.WorldTransformation);
            });

            // Draw part path...
            _primitiveBatch.DrawPrimitive(
                _primitives[shipPart.Configuration].Path, 
                Color.Lerp(shipPart.Color, ShipPartRenderService.TransparentWhite, 0.25f), 
                shipPart.WorldTransformation);
        }
        #endregion

        #region Helper Methods
        public void ValidateConfiguration(ShipPart shipPart)
        {
            if (_primitives.ContainsKey(shipPart.Configuration))
                return;

            _primitives[shipPart.Configuration] = new ShipPartConfigurationPrimitiveData()
            {
                Path = PrimitivePath.Create(_width, shipPart.Configuration.Hull),
                Shapes = shipPart.Configuration.Vertices.Select(v => PrimitiveShape.Create(v)).ToArray()
            };
        }
        #endregion
    }
}
