using Guppy.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    /// <summary>
    /// Driver for the pointer entity that integrates 
    /// the pointer control with the monogame mouse
    /// api.
    /// </summary>
    public class MousePointerDriver : Driver
    {
        private Pointer _pointer;
        private GraphicsDevice _graphics;
        private FarseerCamera2D _camera;

        private Single _oldScrollValue;

        public MousePointerDriver(Pointer pointer, GraphicsDevice graphics, FarseerCamera2D camera, IServiceProvider provider, ILogger logger) : base(pointer, provider, logger)
        {
            _pointer = pointer;
            _graphics = graphics;
            _camera = camera;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var mState = Mouse.GetState();

            var mPos = _graphics.Viewport.Unproject(
                new Vector3(mState.Position.X, mState.Position.Y, 0), 
                _camera.Projection, 
                _camera.View,
                _camera.World);

            // Update the pointer position
            _pointer.MoveTo(mPos.X, mPos.Y);

            // Update the camera zoom value
            var scrollDelta = mState.ScrollWheelValue - _oldScrollValue;
            _camera.ZoomBy(1 + (0.1f * ((Single)scrollDelta / 120)));
            _oldScrollValue = mState.ScrollWheelValue;
        }
    }
}
