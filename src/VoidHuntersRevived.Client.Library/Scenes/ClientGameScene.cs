using FarseerPhysics.DebugView;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Interfaces;
using Guppy.IO.Commands.Services;
using Guppy.IO.Input;
using Guppy.IO.Services;
using Guppy.LayerGroups;
using Guppy.UI.Entities;
using Guppy.UI.Layers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Enums;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Layers;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class ClientGameScene : GameScene
    {
        #region Private Fields
        private ShipPartRenderService _shipPartRenderService;

        private GameWindow _window;
        private GraphicsDevice _graphics;
        private FarseerCamera2D _camera;
        private MouseService _mouse;
        private ContentManager _content;
        private Texture2D[] _backgrounds;
        private SpriteBatch _spriteBatch;
        private TrailService _trails;

        private Vector2 _viewportSize;
        private Rectangle _viewportBounds;

        private DebugViewXNA _debugMaster;
        private DebugViewXNA _debugSlave;
        private Boolean _renderMaster;
        private Boolean _renderSlave;

        private CommandService _commands;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _shipPartRenderService);
            provider.Service(out _window);
            provider.Service(out _graphics);
            provider.Service(out _camera);
            provider.Service(out _mouse);
            provider.Service(out _content);
            provider.Service(out _spriteBatch);
            provider.Service(out _commands);
            provider.Service(out _trails);

            _backgrounds = new Texture2D[]
            {
                provider.GetContent<Texture2D>("sprite:background:1"),
                provider.GetContent<Texture2D>("sprite:background:2"),
                provider.GetContent<Texture2D>("sprite:background:3")
            };

            _camera.MinZoom = 0.025f;
            _camera.MaxZoom = 0.5f;

            // Pre world updates (Cursor) 
            this.Layers.Create<GameLayer>((l, p, c) =>
            {
                l.Group = new SingleLayerGroup(-10);
                l.DrawOrder = -10;
                l.UpdateOrder = -10;
            });

            // Create a ScreenLayer to hold the stage..
            this.Layers.Create<ScreenLayer>((l, p, c) =>
            {
                l.Group = new SingleLayerGroup(1);
                l.DrawOrder = 20;
            });

            this.Entities.Create<Stage>((s, p, d) =>
            {
                s.LayerGroup = 1;
            });

            _commands["toggle"]["debug"].OnExcecute += this.HandleToggleDebugCommand;

            _mouse.OnScrollWheelValueChanged += this.HandleMouseScrollWheelValueChanged;
            _window.ClientSizeChanged += this.HandleClientSizeChanged;

            this.IfOrOnWorld(world =>
            { // Setup world rendering after a world instance is created
                _debugMaster = new DebugViewXNA(world.Master);
                _debugMaster.LoadContent(_graphics, _content);

                _debugSlave = new DebugViewXNA(world.Slave);
                _debugSlave.LoadContent(_graphics, _content);

                _debugSlave.InactiveShapeColor = Color.Green;
                _debugSlave.DefaultShapeColor = Color.Green;
            });

            this.CleanViewport();
        }

        protected override void Release()
        {
            base.Release();

            _commands["toggle"]["debug"].OnExcecute -= this.HandleToggleDebugCommand;

            _mouse.OnScrollWheelValueChanged -= this.HandleMouseScrollWheelValueChanged;
            _window.ClientSizeChanged -= this.HandleClientSizeChanged;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _shipPartRenderService.Update(gameTime);

            _trails.TryUpdate(gameTime);
        }

        protected override void PreDraw(GameTime gameTime)
        {
            base.PreDraw(gameTime);

            _camera.TryClean(gameTime);
            _graphics.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.LinearWrap, blendState: BlendState.AlphaBlend);
            for (Int32 i = 0; i < _backgrounds.Length; i++)
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

            _trails.TryDraw(gameTime);
        }

        private void DrawMaster(GameTime gameTime)
            => _debugMaster.RenderDebugData(_camera.Projection, _camera.View);

        private void DrawSlave(GameTime gameTime)
            => _debugSlave.RenderDebugData(_camera.Projection, _camera.View);
        #endregion

        #region Helper Methods
        private void CleanViewport()
        {
            _viewportBounds = new Rectangle(_graphics.Viewport.Width, -_graphics.Viewport.Height, _graphics.Viewport.Width * 3, _graphics.Viewport.Height * 3);
            _viewportSize = new Vector2(-_graphics.Viewport.Width, -_graphics.Viewport.Height);
        }
        #endregion

        #region Command Handlers
        private CommandResponse HandleToggleDebugCommand(ICommand sender, CommandInput input)
        {
            try
            {
                switch ((DebugType)input["type"])
                {
                    case DebugType.Master:
                        _renderMaster = !_renderMaster;
                        this.IfOrOnWorld(w =>
                        { // Ensure that the world exists before this stage...
                        if (_renderMaster)
                                this.OnDraw += this.DrawMaster;
                            else
                                this.OnDraw -= this.DrawMaster;
                        });

                        return CommandResponse.Success($"Set RenderMaster to {_renderMaster}.");
                    case DebugType.Slave:
                        _renderSlave = !_renderSlave;
                        this.IfOrOnWorld(w =>
                        { // Ensure that the world exists before this stage...
                        if (_renderSlave)
                                this.OnDraw += this.DrawSlave;
                            else
                                this.OnDraw -= this.DrawSlave;
                        });

                        return CommandResponse.Success($"Set RenderSlave to {_renderSlave}.");
                    default:
                        return CommandResponse.Empty;
                }
            }
            catch(Exception e)
            {
                return CommandResponse.Error($"Unable to toggle Debug Render.", e);
            }
        }
        #endregion

        #region Event Handlers
        private void HandleClientSizeChanged(object sender, EventArgs e)
            => this.CleanViewport();

        private void HandleMouseScrollWheelValueChanged(MouseService sender, ScrollWheelArgs args)
            => _camera.ZoomBy((Single)Math.Pow(1.5, args.Delta / 120));
        #endregion
    }
}
