using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Extensions.DependencyInjection;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Interfaces;
using Guppy.IO.Commands.Services;
using Guppy.IO.Input;
using Guppy.IO.Input.Services;
using Guppy.IO.Services;
using Guppy.LayerGroups;
using Guppy.UI.Entities;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Enums;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Drivers;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Layers;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Drivers.Scenes
{
    internal sealed class GameSceneGraphicsDriver : BaseAuthorizationDriver<GameScene>
    {
        #region Private Attributes
        private GameWindow _window;
        private GraphicsDevice _graphics;
        private BasicEffect _ambient;
        private FarseerCamera2D _camera;
        private MouseService _mouse;
        private ContentManager _content;
        private Guppy.Utilities.PrimitiveBatch _primitiveBatch;
        private Sensor _sensor;
        private ChunkManager _chunks;
        private Texture2D[] _backgrounds;
        private SpriteBatch _spriteBatch;
        private GameScene _scene;

        private Vector2 _viewportSize;
        private Rectangle _viewportBounds;

        private DebugViewXNA _debugMaster;
        private DebugViewXNA _debugSlave;
        private Boolean _renderMaster;
        private Boolean _renderSlave;

        private CommandService _commands;
        #endregion

        #region Lifecycle Methods
        protected override void ConfigureMinimum(ServiceProvider provider)
        {
            base.ConfigureMinimum(provider);

            // Create new required layers
            // Pre world updates (Cursor) 
            this.driven.Layers.Create<GameLayer>((l, p, c) =>
            {
                l.Group = new SingleLayerGroup(-10);
                l.DrawOrder = -10;
                l.UpdateOrder = -10;
            });

            // Post world components (Sensor, TractorBeam, Ship, TrailManager, ect)
            this.driven.Layers.Create<GameLayer>((l, p, c) =>
            {
                l.Group = new SingleLayerGroup(10);
                l.DrawOrder = 10;
                l.UpdateOrder = 10;
            });

            provider.Service(out _window);
            provider.Service(out _graphics);
            provider.Service(out _camera);
            provider.Service(out _mouse);
            provider.Service(out _content);
            provider.Service(out _primitiveBatch);
            provider.Service(out _sensor);
            provider.Service(out _chunks);
            provider.Service(out _spriteBatch);
            provider.Service(out _commands);
            provider.Service(out _scene);

            _ambient = new BasicEffect(_graphics);

            _camera.MinZoom = 0.025f;
            _camera.MaxZoom = 0.5f;

            _mouse.OnScrollWheelValueChanged += this.HandleMouseScrollWheelValueChanged;
            this.driven.OnPreDraw += this.PreDraw;
            _window.ClientSizeChanged += this.HandleClientSizeChanged;
            _commands["toggle"]["debug"].OnExcecute += this.HandleToggleDebugCommands;

            _scene.IfOrOnWorld(world =>
            { // Setup world rendering after a world instance is created
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

            this.CleanViewport();
        }

        protected override void ReleaseMinimum()
        {
            base.ReleaseMinimum();

            this.driven.OnPreDraw -= this.PreDraw;
            _mouse.OnScrollWheelValueChanged -= this.HandleMouseScrollWheelValueChanged;
            _window.ClientSizeChanged -= this.HandleClientSizeChanged;
            _commands["toggle"]["debug"].OnExcecute -= this.HandleToggleDebugCommands;
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

        private void DrawMaster(GameTime gameTime)
        {
            _debugMaster.RenderDebugData(_camera.Projection, _camera.View);
        }

        private void DrawSlave(GameTime gameTime)
        {
            _debugSlave.RenderDebugData(_camera.Projection, _camera.View);
        }
        #endregion

        #region EventHandlers
        private void HandleMouseScrollWheelValueChanged(MouseService sender, ScrollWheelArgs args)
            => _camera.ZoomBy((Single) Math.Pow(1.5, args.Delta / 120));

        private void HandleClientSizeChanged(object sender, EventArgs e)
            => this.CleanViewport();

        private void HandleToggleDebugCommands(ICommand sender, CommandArguments args)
        {
            switch ((DebugType)args["type"])
            {
                case DebugType.Master:
                    _renderMaster = !_renderMaster;
                    _scene.IfOrOnWorld(w =>
                    { // Ensure that the world exists before this stage...
                        if (_renderMaster)
                            this.driven.OnDraw += this.DrawMaster;
                        else
                            this.driven.OnDraw -= this.DrawMaster;
                    });
                    break;
                case DebugType.Slave:
                    _renderSlave = !_renderSlave;
                    _scene.IfOrOnWorld(w =>
                    { // Ensure that the world exists before this stage...
                        if (_renderSlave)
                            this.driven.OnDraw += this.DrawSlave;
                        else
                            this.driven.OnDraw -= this.DrawSlave;
                    });
                    break;
            }
        }
        #endregion
    }
}
