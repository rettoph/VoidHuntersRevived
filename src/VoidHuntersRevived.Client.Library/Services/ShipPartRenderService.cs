using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Utilities;
using Guppy.Utilities.Primitives;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
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
    public class ShipPartRenderService : Asyncable
    {
        #region Private Fields
        private Dictionary<ShipPartConfiguration, PrimitivePath> _paths;

        private Single _configuredZoom;
        private FarseerCamera2D _camera;
        private PrimitiveBatch _primitiveBatch;
        private Single _width = 1;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            _paths = new Dictionary<ShipPartConfiguration, PrimitivePath>();

            provider.Service(out _camera);
            provider.Service(out _primitiveBatch);
        }

        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);

            this.TryStart(false);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(_camera.Zoom != _configuredZoom && Math.Abs(_configuredZoom - _camera.Zoom) / _configuredZoom > 0.005f)
            {
                _configuredZoom = _camera.Zoom;
                _width = 0.01f * (1 / _camera.Zoom);

                foreach(PrimitivePath path in _paths.Values)
                    path.Width = _width;
            }
        }

        public void Draw(ShipPart shipPart)
        {
            _primitiveBatch.DrawPrimitive(
                _paths[shipPart.Configuration], 
                shipPart.Color, 
                shipPart.WorldTransformation);
        }
        #endregion

        #region Helper Methods
        public void ValidateConfiguration(ShipPart shipPart)
        {
            if (_paths.ContainsKey(shipPart.Configuration))
                return;

            _paths[shipPart.Configuration] = PrimitivePath.Create(_width, shipPart.Configuration.Hull);
        }
        #endregion
    }
}
