using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Drivers.Scenes
{
    /// <summary>
    /// Renders client specific data for the world scene
    /// </summary>
    [IsDriver(typeof(WorldScene), 1000)]
    public sealed class WorldSceneRenderDriver : Driver<WorldScene>
    {
        #region Private Fields
        private FarseerCamera2D _camera;
        private GraphicsDevice _graphics;
        private ContentManager _contentManager;
        private ContentLoader _content;
        private World _world;
        private DebugViewXNA _debug;
        private ServerShadow _shadow;
        private DebugViewXNA _debugShadow;
        private SpriteBatch _spriteBatch;

        private Texture2D _background01;
        private Texture2D _background02;
        private Texture2D _background03;
        #endregion

        #region Constructor
        public WorldSceneRenderDriver(ContentLoader content, FarseerCamera2D camera, GraphicsDevice graphics, ContentManager contentManager, World world, ServerShadow shadow, WorldScene driven) : base(driven)
        {
            _camera = camera;
            _graphics = graphics;
            _contentManager = contentManager;
            _world = world;
            _shadow = shadow;
            _content = content;
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            _debug = new DebugViewXNA(_world);
            _debug.LoadContent(_graphics, _contentManager);

            _debugShadow = new DebugViewXNA(_shadow.World);
            _debugShadow.LoadContent(_graphics, _contentManager);

            _debugShadow.DefaultShapeColor = Color.Lerp(_debugShadow.DefaultShapeColor, Color.Red, 0.5f);
            _debugShadow.InactiveShapeColor = Color.Lerp(_debugShadow.InactiveShapeColor, Color.Red, 0.5f);
            _debugShadow.KinematicShapeColor = Color.Lerp(_debugShadow.KinematicShapeColor, Color.Red, 0.5f);
            _debugShadow.SleepingShapeColor = Color.Lerp(_debugShadow.SleepingShapeColor, Color.Red, 0.5f);
            _debugShadow.StaticShapeColor = Color.Lerp(_debugShadow.StaticShapeColor, Color.Red, 0.5f);
            _debugShadow.TextColor = Color.Lerp(_debugShadow.TextColor, Color.Red, 0.05f);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _background01 = _content.TryGet<Texture2D>("sprite:background:1");
            _background02 = _content.TryGet<Texture2D>("sprite:background:2");
            _background03 = _content.TryGet<Texture2D>("sprite:background:3");

            _spriteBatch = new SpriteBatch(_graphics);
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            
            // Clear the graphics device
            _graphics.Clear(new Color(0, 0, 10));


            var bounds = new Rectangle(_graphics.Viewport.Width, -_graphics.Viewport.Height, _graphics.Viewport.Width * 3, _graphics.Viewport.Height * 3);

            _spriteBatch.Begin(samplerState: SamplerState.LinearWrap, blendState: BlendState.AlphaBlend);

            _spriteBatch.Draw(_background01, new Vector2(-_graphics.Viewport.Width, -_graphics.Viewport.Height) + new Vector2(-_camera.Position.X * 1 % _background01.Width, -_camera.Position.Y * 1 % _background01.Height), bounds, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            _spriteBatch.Draw(_background02, new Vector2(-_graphics.Viewport.Width, -_graphics.Viewport.Height) + new Vector2(-_camera.Position.X * 2 % _background02.Width, -_camera.Position.Y * 2 % _background02.Height), bounds, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            _spriteBatch.Draw(_background03, new Vector2(-_graphics.Viewport.Width, -_graphics.Viewport.Height) + new Vector2(-_camera.Position.X * 3 % _background03.Width, -_camera.Position.Y * 3 % _background03.Height), bounds, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            _spriteBatch.End();

            _debugShadow.RenderDebugData(_camera.Projection, _camera.View);
            _debug.RenderDebugData(_camera.Projection, _camera.View);
        }
        #endregion
    }
}
