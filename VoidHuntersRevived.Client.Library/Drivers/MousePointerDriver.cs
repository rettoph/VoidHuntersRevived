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
        private FarseerCamera2D _camera;

        private Single _oldScrollValue;

        public MousePointerDriver(FarseerCamera2D camera, Pointer pointer, IServiceProvider provider, ILogger logger) : base(pointer, provider, logger)
        {
            _pointer = pointer;
            _camera = camera;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var mState = Mouse.GetState();

            // var mPos = _graphics.Viewport.Unproject(
            //     new Vector3(mState.Position.X, mState.Position.Y, 0), 
            //     _camera.Projection, 
            //     _camera.View,
            //     _camera.World);

            // Update the pointer position
            _pointer.MoveTo(mState.Position.X, mState.Position.Y);

            // Update the primary and secondary values
            _pointer.SetPrimary(mState.LeftButton == ButtonState.Pressed);
            _pointer.SetSecondary(mState.RightButton == ButtonState.Pressed);

            // Update the camera zoom value
            var scrollDelta = mState.ScrollWheelValue - _oldScrollValue;
            _camera.ZoomBy(1 + (0.1f * ((Single)scrollDelta / 120)));
            _oldScrollValue = mState.ScrollWheelValue;
        }
    }
}
