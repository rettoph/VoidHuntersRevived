using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Drivers.Scenes
{
    /// <summary>
    /// Simple driver used to update the client's camera each frame
    /// </summary>
    [IsDriver(typeof(WorldScene), 110)]
    internal sealed class WorldSceneCameraDriver : Driver<WorldScene>
    {
        #region Private Fields
        private FarseerCamera2D _camera;
        #endregion

        #region Constructor
        public WorldSceneCameraDriver(FarseerCamera2D camera, WorldScene driven) : base(driven)
        {
            _camera = camera;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _camera.TryUpdate(gameTime);
        }
        #endregion
    }
}
