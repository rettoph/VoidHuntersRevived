﻿using Guppy.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Scenes;
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
        private VoidHuntersClientWorldScene _scene;
        private Pointer _pointer;
        private FarseerCamera2D _camera;

        private Single _oldScrollValue;

        public MousePointerDriver(VoidHuntersClientWorldScene scene, FarseerCamera2D camera, Pointer pointer, IServiceProvider provider) : base(pointer, provider)
        {
            _scene = scene;
            _pointer = pointer;
            _camera = camera;
        }

        protected override void update(GameTime gameTime)
        {
            base.update(gameTime);

            var mState = Mouse.GetState();

            // Update the pointer position
            _pointer.MoveTo(mState.Position.X, mState.Position.Y);

            // Update the primary and secondary values
            _pointer.SetPrimary(mState.LeftButton == ButtonState.Pressed);
            _pointer.SetSecondary(mState.RightButton == ButtonState.Pressed);

            // Update the scroll value
            _pointer.ScrollTo(mState.ScrollWheelValue);
        }
    }
}
