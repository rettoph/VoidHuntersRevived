using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Attributes;
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
        private ContentManager _content;
        private World _world;
        private DebugViewXNA _debug;
        private ServerShadow _shadow;
        private DebugViewXNA _debugShadow;
        #endregion

        #region Constructor
        public WorldSceneRenderDriver(FarseerCamera2D camera, GraphicsDevice graphics, ContentManager content, World world, ServerShadow shadow, WorldScene driven) : base(driven)
        {
            _camera = camera;
            _graphics = graphics;
            _content = content;
            _world = world;
            _shadow = shadow;
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            _debug = new DebugViewXNA(_world);
            _debug.LoadContent(_graphics, _content);

            _debugShadow = new DebugViewXNA(_shadow.World);
            _debugShadow.LoadContent(_graphics, _content);

            _debugShadow.DefaultShapeColor = Color.Red;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            
            // Clear the graphics device
            _graphics.Clear(Color.Black);

            _debugShadow.RenderDebugData(_camera.Projection, _camera.View);
            _debug.RenderDebugData(_camera.Projection, _camera.View);
        }
        #endregion
    }
}
