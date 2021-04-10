using System;
using System.Linq;
using Guppy.IO.Input;
using Guppy.IO.Services;
using Guppy.UI.Entities;
using Guppy.UI.Layers;
using Guppy.LayerGroups;
using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Layers;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class ClientGameScene : GraphicsGameScene
    {
        #region Private Fields
        private MouseService _mouse;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _mouse);

            this.camera.MinZoom = 5f;
            this.camera.MaxZoom = 40f;

            // Pre world updates (Cursor) 
            this.Layers.Create<GameLayer>((l, p, c) =>
            {
                l.Group = new SingleLayerGroup(-10);
                l.DrawOrder = -10;
                l.UpdateOrder = -10;
            });

            _mouse.OnScrollWheelValueChanged += this.HandleMouseScrollWheelValueChanged;
        }

        protected override void Release()
        {
            base.Release();

            _mouse.OnScrollWheelValueChanged -= this.HandleMouseScrollWheelValueChanged;

            _mouse = null;
        }
        #endregion

        #region Event Handlers
        private void HandleMouseScrollWheelValueChanged(MouseService sender, ScrollWheelArgs args)
            => this.camera.ZoomBy((Single)Math.Pow(1.5, args.Delta / 120));
        #endregion
    }
}
