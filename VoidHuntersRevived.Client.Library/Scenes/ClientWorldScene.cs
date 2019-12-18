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
        #endregion

        #region Constructor
        public ClientWorldScene(DebugOverlay debug)
        {
            _debug = debug;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _debug.AddLine(gt => $" Action => T: {this.actionCount.ToString("#,##0")}, APS: {(this.actionCount / gt.TotalGameTime.TotalSeconds).ToString("#,##0.000")}");
            _debug.AddLine(gt => $" Vital => T: {VitalsManager.MessagesRecieved.ToString("#,##0")}, VPS: {(VitalsManager.MessagesRecieved / gt.TotalGameTime.TotalSeconds).ToString("#,##0.000")}");

            // Layer 0: Default
            this.layers.Create<CameraLayer>(0, l =>
            {
                l.SetUpdateOrder(10);
                l.SetDrawOrder(20);
            });
            // Layer 1: Chunk
            this.layers.Create<CameraLayer>(1, l =>
            {
                l.SetUpdateOrder(20);
                l.SetDrawOrder(10);
            });
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.layers.TryDraw(gameTime);

            _debug.TryDraw(gameTime);
        }
        #endregion
    }
}
