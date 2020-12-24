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
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Layers;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class ClientGameScene : GraphicsGameScene
    {
        #region Private Fields
        private MouseService _mouse;
        private DebugService _debug;

        private ServiceFactory _bulletServiceFactory;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _mouse);
            provider.Service(out _debug);

            _bulletServiceFactory = provider.GetServiceFactory(typeof(Bullet));

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
            _debug.Lines += this.RenderDebugLines;
        }

        protected override void Release()
        {
            base.Release();

            _mouse.OnScrollWheelValueChanged -= this.HandleMouseScrollWheelValueChanged;
            _debug.Lines -= this.RenderDebugLines;
        }
        #endregion

        #region Event Handlers
        private void HandleMouseScrollWheelValueChanged(MouseService sender, ScrollWheelArgs args)
            => this.camera.ZoomBy((Single)Math.Pow(1.5, args.Delta / 120));

        private string RenderDebugLines(GameTime gameTime)
        {
            return $"Entities: {this.Entities.Count()}\n"
                + $"Bullet Queue: {_bulletServiceFactory.Pools[typeof(Bullet)].Count()}";
        }
        #endregion
    }
}
