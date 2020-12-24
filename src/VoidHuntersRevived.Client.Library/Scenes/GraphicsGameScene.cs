using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Extensions.Utilities;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Interfaces;
using Guppy.IO.Commands.Services;
using Guppy.LayerGroups;
using Guppy.Services;
using Guppy.UI.Entities;
using Guppy.UI.Layers;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Diagnostics;
using VoidHuntersRevived.Client.Library.Effects;
using VoidHuntersRevived.Client.Library.Enums;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    /// <summary>
    /// An implementation of <see cref="GameScene"/>
    /// that will add required graphics functionality.
    /// This is used for both the real game and the main
    /// menu.
    /// </summary>
    public class GraphicsGameScene : GameScene
    {
        #region Private Fields
        private ShipPartRenderService _shipParts;
        private TrailRenderService _trails;
        private ExplosionRenderService _explosions;
        private CommandService _commands;

        private GameWindow _window;
        private GraphicsDevice _graphics;
        private ContentService _content;
        private SpriteBatch _spriteBatch;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private Camera2D _camera;
        private GaussianBlurFilter _blur;

        private Texture2D[] _backgrounds;
        private Vector2 _viewportSize;
        private Rectangle _viewportBounds;
        private Stage _stage;

        private DebugView _debugMaster;
        private DebugView _debugSlave;
        private Boolean _renderMaster;
        private Boolean _renderSlave;
        private Boolean _renderImpulse;
        #endregion

        #region Protected Properties
        protected Camera2D camera => _camera;
        protected Stage stage => _stage;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _shipParts);
            provider.Service(out _trails);
            provider.Service(out _explosions);
            provider.Service(out _commands);
            provider.Service(out _window);
            provider.Service(out _graphics);
            provider.Service(out _content);
            provider.Service(out _spriteBatch);
            provider.Service(out _primitiveBatch);
            provider.Service(out _camera);

            _blur = new GaussianBlurFilter(provider);
            _backgrounds = new Texture2D[]
            {
                provider.GetContent<Texture2D>("sprite:background:1"),
                provider.GetContent<Texture2D>("sprite:background:2"),
                provider.GetContent<Texture2D>("sprite:background:3")
            };

            // Create a ScreenLayer to hold the stage..
            this.Layers.Create<ScreenLayer>((l, p, c) =>
            {
                l.Group = new SingleLayerGroup(1);
                l.DrawOrder = 20;
            });

            _stage = this.Entities.Create<Stage>((s, p, d) =>
            {
                s.LayerGroup = 1;
            });
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.IfOrOnWorld(world =>
            { // Setup world rendering after a world instance is created
                _debugMaster = new DebugView(world.Master);
                _debugMaster.LoadContent(_graphics, _content);

                _debugSlave = new DebugView(world.Live);
                _debugSlave.LoadContent(_graphics, _content);

                _debugSlave.InactiveShapeColor = Color.Green;
                _debugSlave.DefaultShapeColor = Color.Green;
            });

            _window.ClientSizeChanged += this.HandleClientSizeChanged;
            _commands["toggle"]["debug"].OnExcecute += this.HandleToggleDebugCommand;

            // Setup required game instances.
            this.CleanViewport();
            this.CleanEffects();
        }

        protected override void Release()
        {
            base.Release();

            _window.ClientSizeChanged -= this.HandleClientSizeChanged;
            _commands["toggle"]["debug"].OnExcecute -= this.HandleToggleDebugCommand;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the required services...
            _shipParts.TryUpdate(gameTime);
            _trails.TryUpdate(gameTime);
            _explosions.TryUpdate(gameTime);
        }

        protected override void PreDraw(GameTime gameTime)
        {
            base.PreDraw(gameTime);

            _camera.TryClean(gameTime);
            _graphics.Clear(Color.Transparent);

            _blur.Start();
            _spriteBatch.Begin(samplerState: SamplerState.LinearWrap, blendState: BlendState.AlphaBlend);

            for (Int32 i = 0; i < _backgrounds.Length; i++)
                _spriteBatch.Draw(
                    texture: _backgrounds[i],
                    position: _viewportSize + new Vector2(-_camera.Position.X * i % _backgrounds[i].Width, -_camera.Position.Y * i % _backgrounds[0].Height),
                    sourceRectangle: _viewportBounds,
                    color: Color.White,
                    rotation: 0,
                    origin: Vector2.Zero,
                    scale: 1f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);

            _spriteBatch.End();

            _trails.TryDraw(gameTime);
            _explosions.TryDraw(gameTime);

            _blur.End();
        }

        private void DrawMaster(GameTime gameTime)
            => _debugMaster.RenderDebugData(_camera.Projection, _camera.View);

        private void DrawSlave(GameTime gameTime)
            => _debugSlave.RenderDebugData(_camera.Projection, _camera.View);

        private void DrawImpulse(GameTime gameTime)
        {
            _primitiveBatch.Begin(_camera);
            this.Entities.Where(e => e is Thruster).Select(t => t as Thruster).ForEach(t =>
            {
                t.Root.Do(b =>
                {
                    var force = t.Impulse.RotateTo(b.Rotation + t.LocalRotation);
                    var point = b.Position + Vector2.Transform(Vector2.Zero, t.LocalTransformation * Matrix.CreateRotationZ(b.Rotation));

                    _primitiveBatch.DrawLine(Color.Red, point, Color.Transparent, point - force);
                });
            });
            _primitiveBatch.End();
        }
        #endregion

        #region Helper Methods
        private void CleanViewport()
        {
            _viewportBounds = new Rectangle(_graphics.Viewport.Width, -_graphics.Viewport.Height, _graphics.Viewport.Width * 3, _graphics.Viewport.Height * 3);
            _viewportSize = new Vector2(-_graphics.Viewport.Width, -_graphics.Viewport.Height);

            this.CleanEffects();
        }

        private void CleanEffects()
        {
            _blur.Resolution = _graphics.Viewport.Bounds.Size;
            _blur.BlurAmount = 1f;
            _blur.Passes = 1;
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
                    case DebugType.Impulse:
                        _renderImpulse = !_renderImpulse;
                        if (_renderImpulse)
                            this.OnPostDraw += this.DrawImpulse;
                        else
                            this.OnPostDraw -= this.DrawImpulse;
                        return CommandResponse.Success($"Set RenderImpulse to {_renderImpulse}.");
                    default:
                        return CommandResponse.Empty;
                }
            }
            catch (Exception e)
            {
                return CommandResponse.Error($"Unable to toggle Debug Render.", e);
            }
        }
        #endregion

        #region Event Handlers
        private void HandleClientSizeChanged(object sender, EventArgs e)
            => this.CleanViewport();
        #endregion
    }
}
