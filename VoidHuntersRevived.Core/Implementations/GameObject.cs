using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Implementations
{
    public abstract class GameObject : Initializable, IGameObject
    {
        private Boolean _visible;
        private Boolean _enabled;
        private Int32 _drawOrder;
        private Int32 _updateOrder;

        public Boolean Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                this.VisibleChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public Boolean Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                this.EnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public Int32 DrawOrder
        {
            get { return _drawOrder; }
            set
            {
                _drawOrder = value;
                this.DrawOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public Int32 UpdateOrder
        {
            get { return _updateOrder; }
            set
            {
                _updateOrder = value;
                this.UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public IGame Game { get; private set; }

        public GameObject(IGame game) : base(game.Logger)
        {
            this.Game = game;
        }

        public abstract void Draw(GameTime gameTime);
        public abstract void Update(GameTime gameTime);

        public void SetEnabled(bool enabled)
        {
            this.Enabled = enabled;
        }

        public void SetVisible(bool visible)
        {
            this.Visible = visible;
        }
    }
}
