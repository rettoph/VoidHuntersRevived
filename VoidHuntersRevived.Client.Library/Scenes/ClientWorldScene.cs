using Guppy.UI.Components;
using Guppy.Utilities.Cameras;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Layers;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Server.Utilities;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class ClientWorldScene : WorldScene
    {
        #region Private Fields
        private DebugOverlay _debug;
        private IServiceProvider _provider;
        private Camera2D _camera;
        #endregion

        #region Constructor
        public ClientWorldScene(IServiceProvider provider, DebugOverlay debug)
        {
            _debug = debug;
            _provider = provider;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _camera = _provider.GetRequiredService<Camera2D>();
            _camera.Center = false;
            _camera.MoveBy(new Vector2(0.5f, 0.5f));

            _debug.AddLine(gt => $" Action => T: {this.actionCount.ToString("#,##0")}, APS: {(this.actionCount / gt.TotalGameTime.TotalSeconds).ToString("#,##0.000")}");
            _debug.AddLine(gt => $" Vital => T: {VitalsManager.MessagesRecieved.ToString("#,##0")}, VPS: {(VitalsManager.MessagesRecieved / gt.TotalGameTime.TotalSeconds).ToString("#,##0.000")}");
            _debug.AddLine(gt => $"\n{(1/gt.ElapsedGameTime.TotalSeconds).ToString("FPS: #,##0.0")}");

            // Layer 0: Default
            this.layers.Create<CameraLayer>(0, l =>
            {
                l.UpdateOrder = 10;
                l.DrawOrder = 20;
            });
            // Layer 1: Chunk
            this.layers.Create<CameraLayer>(1, l =>
            {
                l.UpdateOrder = 20;
                l.DrawOrder = 10;
            });
            // Layer 2: Trails
            this.layers.Create<PrimitiveLayer>(2, l =>
            {
                l.UpdateOrder = 20;
                l.DrawOrder = 15;
            });
            // Layer 3: Static Elements (Popups, Energy Bar ect..)
            this.layers.Create<PrimitiveLayer>(3, l =>
            {
                l.UpdateOrder = 20;
                l.DrawOrder = 30;
                l.SetCamera(_camera);
            });
            // Layer 4: UI
            this.layers.Create<PrimitiveLayer>(4, l =>
            {
                l.UpdateOrder = 20;
                l.DrawOrder = 40;
                l.SetCamera(_camera);
            });
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            _camera.TryUpdate(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _camera.TryDraw(gameTime);

            this.layers.TryDraw(gameTime);

            _debug.TryDraw(gameTime);
        }
        #endregion
    }
}
