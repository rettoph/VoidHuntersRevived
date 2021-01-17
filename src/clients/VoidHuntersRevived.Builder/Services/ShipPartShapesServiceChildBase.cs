﻿using Guppy;
using Guppy.DependencyInjection;
using Guppy.IO.Services;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Builder.Services
{
    /// <summary>
    /// A simple help class to manage default shape required
    /// values.
    /// </summary>
    public abstract class ShipPartShapesServiceChildBase : Frameable
    {
        #region Protected Properties
        protected internal ShipPartShapesBuilderService shapes { get; internal set; }
        protected GraphicsDevice graphics { get; private set; }
        protected SpriteBatch spriteBatch { get; private set; }
        protected PrimitiveBatch<VertexPositionColor> primitiveBatch { get; private set; }
        protected Synchronizer synchronizer { get; private set; }
        protected LockService @lock { get; private set; }
        protected Camera2D camera { get; private set; }
        protected MouseService mouse { get; private set; }
        protected virtual Vector2 mouseWorldPosition => this.camera.Unproject(this.mouse.Position);
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.graphics = provider.GetService<GraphicsDevice>();
            this.spriteBatch = provider.GetService<SpriteBatch>();
            this.primitiveBatch = provider.GetService<PrimitiveBatch<VertexPositionColor>>();
            this.synchronizer = provider.GetService<Synchronizer>();
            this.@lock = provider.GetService<LockService>();
            this.camera = provider.GetService<Camera2D>();
            this.mouse = provider.GetService<MouseService>();
        }

        protected override void Release()
        {
            base.Release();

            this.graphics = null;
            this.spriteBatch = null;
            this.primitiveBatch = null;
            this.synchronizer = null;
            this.@lock = null;
            this.camera = null;
            this.mouse = null;
        }
        #endregion
    }
}
