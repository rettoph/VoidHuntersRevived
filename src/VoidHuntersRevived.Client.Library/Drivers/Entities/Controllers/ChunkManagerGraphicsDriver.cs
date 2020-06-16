using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Extensions.System.Drawing;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.Controllers
{
    internal sealed class ChunkManagerGraphicsDriver : Driver<ChunkManager>
    {
        #region Static Attributes
        public static Microsoft.Xna.Framework.Color ChunkColor { get; set; } = new Microsoft.Xna.Framework.Color(Microsoft.Xna.Framework.Color.DarkSlateBlue, 40);
        #endregion

        #region Private Fields
        private FarseerCamera2D _camera;
        private WorldEntity _world;
        private PrimitiveBatch _primitiveBatch;
        #endregion

        #region Lifecycle Methods
        protected override void Configure(object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            provider.Service(out _camera);
            provider.Service(out _primitiveBatch);

            provider.GetService<GameScene>().IfOrOnWorld(w =>
            {
                _world = w;
                this.driven.OnDraw += this.Draw;
            });
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.driven.OnDraw -= this.Draw;
        }
        #endregion

        #region Frame Methods
        private void Draw(GameTime gameTime)
        {
            var hw = ((_camera.ViewportBounds.Width / 2) / _camera.Zoom) + Chunk.Size;
            var hh = ((_camera.ViewportBounds.Height / 2) / _camera.Zoom) + Chunk.Size;

            var bounds = new RectangleF()
            {
                X = _camera.Position.X - hw,
                Y = _camera.Position.Y - hh,
                Width = hw * 2,
                Height = hh * 2
            }.Intersection(new RectangleF(0, 0, _world.Size.X, _world.Size.Y));

            // Draw all visible chunks...
            this.driven.GetChunks(bounds).ForEach(c =>
            {
                // _primitiveBatch.DrawRectangle(c.Bounds, ChunkManagerGraphicsDriver.ChunkColor);
                c.TryDraw(gameTime);
            });
        }
        #endregion
    }
}
