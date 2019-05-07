using FarseerPhysics;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using Guppy.UI.Elements;
using Guppy.UI.Entities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Layers;
using VoidHuntersRevived.Client.Players;
using VoidHuntersRevived.Client.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Scenes
{
    public class VoidHuntersClientWorldScene : VoidHuntersWorldScene
    {
        private GraphicsDevice _graphics;
        private DebugViewXNA _debug;
        private SpriteBatch _spriteBatch;
        private ContentManager _content;

        private FarseerCamera2D _camera;

        public VoidHuntersClientWorldScene(FarseerCamera2D camera, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager content, World world, IServiceProvider provider) : base(world, provider)
        {
            _camera = camera;
            _spriteBatch = spriteBatch;
            _graphics = graphicsDevice;
            _content = content;
        }

        protected override void Boot()
        {
            base.Boot();

            _debug = new DebugViewXNA(this.world);
            _debug.LoadContent(_graphics, _content);
        }
        protected override void PreInitialize()
        {
            base.PreInitialize();

            _debug.AppendFlags(DebugViewFlags.ContactPoints);
            _debug.AppendFlags(DebugViewFlags.ContactNormals);
            _debug.AppendFlags(DebugViewFlags.Controllers);

            this.DefaultLayerDepth = 1;

            // this.layers.Create<HudLayer>(0, 0, 0, 1);
            this.layers.Create<CameraLayer>(1, 1, 0, 0);
        }
        protected override void Initialize()
        {
            base.Initialize();

            this.players.Add(
                new LocalPlayer(
                    _camera,
                    this.entities.Create<ShipPart>("entity:ship-part"), 
                    this.logger));

            var part = this.entities.Create<ShipPart>("entity:ship-part");
            part.Body.Position = new Vector2(3, 0);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _camera.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            _graphics.Clear(Color.Black);

            base.Draw(gameTime);

            _spriteBatch.Begin();
            _debug.RenderDebugData(_camera.Projection, _camera.View);
            _spriteBatch.End();
        }
    }
}
