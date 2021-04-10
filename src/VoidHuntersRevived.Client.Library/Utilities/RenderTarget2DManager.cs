using Guppy;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Windows.Library.Utilities
{
    /// <summary>
    /// Simple helper class that can be used as a render target & will
    /// automatically be resized on screensize changed
    /// </summary>
    public sealed class RenderTarget2DManager : IDisposable
    {
        #region Private Fields
        private GraphicsDevice _graphics;
        private GameWindow _window;
        private RenderTarget2D _target;
        #endregion

        #region Public Properties
        public RenderTarget2D Target => _target;
        public Rectangle Bounds => _target.Bounds;
        #endregion

        #region Events
        public OnEventDelegate<RenderTarget2DManager, RenderTarget2D> OnTargetCleaned;
        #endregion

        #region Constructors
        public RenderTarget2DManager(GraphicsDevice graphics, GameWindow window)
        {
            _graphics = graphics;
            _window = window;

            _window.ClientSizeChanged += this.HandleClientSizeChanged;

            this.CleanTarget();
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            _target.Dispose();
            _window.ClientSizeChanged += this.HandleClientSizeChanged;
        }
        #endregion

        #region Helper Methods
        private void CleanTarget()
        {
            _target?.Dispose();
            _target = new RenderTarget2D(_graphics, _window.ClientBounds.Width, _window.ClientBounds.Height);

            this.OnTargetCleaned?.Invoke(this, _target);
        }
        #endregion

        #region Event Handlers
        private void HandleClientSizeChanged(object sender, EventArgs e)
            => this.CleanTarget();
        #endregion

        #region Operators
        public static implicit operator RenderTarget2D(RenderTarget2DManager target)
            => target.Target;
        #endregion
    }
}
