﻿using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Extensions.DependencyInjection;
using Guppy.UI.Entities;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Drivers;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Drivers.Scenes
{
    internal sealed class GameSceneGraphicsDriver : BaseAuthorizationDriver<GameScene>
    {
        #region Private Attributes
        private GameWindow _window;
        private GraphicsDevice _graphics;
        private BasicEffect _ambient;
        private DebugViewXNA _debugMaster;
        private DebugViewXNA _debugSlave;
        private FarseerCamera2D _camera;
        private Cursor _cursor;
        private ContentManager _content;
        private Guppy.Utilities.PrimitiveBatch _primitiveBatch;
        private Sensor _sensor;
        private ChunkManager _chunks;
        private Texture2D[] _backgrounds;
        private SpriteBatch _spriteBatch;

        private Vector2 _viewportSize;
        private Rectangle _viewportBounds;
        #endregion

        #region Lifecycle Methods
        protected override void ConfigurePartial(ServiceProvider provider)
        {
            base.ConfigurePartial(provider);

            provider.Service(out _window);
            provider.Service(out _graphics);
            provider.Service(out _camera);
            provider.Service(out _cursor);
            provider.Service(out _content);
            provider.Service(out _primitiveBatch);
            provider.Service(out _sensor);
            provider.Service(out _chunks);
            provider.Service(out _spriteBatch);

            _ambient = new BasicEffect(_graphics);

            _camera.MinZoom = 0.025f;
            _camera.MaxZoom = 0.5f;

            _cursor.OnScrolled += this.HandleCursorScroll;
            this.driven.OnPreDraw += this.PreDraw;
            this.driven.OnPostDraw += this.PostDraw;
            _window.ClientSizeChanged += this.HandleClientSizeChanged;

            provider.GetService<GameScene>().IfOrOnWorld(world =>
            { // Setup world rendering after a world instance is created
                this.driven.OnDraw += this.Draw;

                _debugMaster = new DebugViewXNA(world.Master);
                _debugMaster.LoadContent(_graphics, _content);

                _debugSlave = new DebugViewXNA(world.Slave);
                _debugSlave.LoadContent(_graphics, _content);

                _debugSlave.InactiveShapeColor = Color.Green;
                _debugSlave.DefaultShapeColor = Color.Green;
            });

            // Load default backgrounds
            _backgrounds = new Texture2D[]
            {
                provider.GetContent<Texture2D>("sprite:background:1"),
                provider.GetContent<Texture2D>("sprite:background:2"),
                provider.GetContent<Texture2D>("sprite:background:3")
            };
        }

        protected override void DisposePartial()
        {
            base.DisposePartial();

            this.driven.OnDraw -= this.Draw;
            this.driven.OnPreDraw -= this.PreDraw;
            this.driven.OnPostDraw -= this.PostDraw;
            _window.ClientSizeChanged -= this.HandleClientSizeChanged;
        }
        #endregion

        #region Helper Methods
        private void CleanViewport()
        {
            _viewportBounds = new Rectangle(_graphics.Viewport.Width, -_graphics.Viewport.Height, _graphics.Viewport.Width * 3, _graphics.Viewport.Height * 3);
            _viewportSize = new Vector2(-_graphics.Viewport.Width, -_graphics.Viewport.Height);
        }
        #endregion

        #region Frame Methods
        private void PreDraw(GameTime gameTime)
        {
            _camera.TryClean(gameTime);
            _primitiveBatch.Begin(_camera.View, _camera.Projection);
            _graphics.Clear(Color.Black);

            
            _spriteBatch.Begin(samplerState: SamplerState.LinearWrap, blendState: BlendState.AlphaBlend);
            for (Int32 i=0; i< _backgrounds.Length; i++)
                _spriteBatch.Draw(
                    _backgrounds[i], 
                    _viewportSize + new Vector2(-_camera.Position.X * i % _backgrounds[i].Width, -_camera.Position.Y * i % _backgrounds[0].Height),
                    _viewportBounds, 
                    Color.White, 
                    0, 
                    Vector2.Zero, 
                    1f, 
                    SpriteEffects.None, 
                    0);
            _spriteBatch.End();
        }

        private void Draw(GameTime gameTime)
        {
            // _debugMaster.RenderDebugData(_camera.Projection, _camera.View);
            // _debugSlave.RenderDebugData(_camera.Projection, _camera.View);
        }

        private void PostDraw(GameTime gameTime)
        {
            _primitiveBatch.End();
        }
        #endregion

        #region EventHandlers
        private void HandleCursorScroll(Cursor sender, float old, float value)
        {
            var delta = (value - old) / 120;
            _camera.ZoomBy((Single)Math.Pow(1.5, delta));
        }

        private void HandleClientSizeChanged(object sender, EventArgs e)
            => this.CleanViewport();
        #endregion
    }
}
