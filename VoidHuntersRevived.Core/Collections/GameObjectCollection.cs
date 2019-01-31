using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Interfaces;
using System.Linq;

namespace VoidHuntersRevived.Core.Collections
{
    public class GameObjectCollection<TGameObject> : InitializableGameCollection<TGameObject>
        where TGameObject : IGameObject
    {
        public Int32 EnabledCount { get { return _updatables.Length; } }
        public Int32 VisibleCount { get { return _drawables.Length; } }

        private TGameObject[] _drawables;
        private TGameObject[] _updatables;

        private Boolean _dirtyDrawables;
        private Boolean _dirtyUpdatables;

        public GameObjectCollection(ILogger logger) : base(logger)
        {
            _dirtyDrawables = true;
            _dirtyUpdatables = true;
        }

        public virtual void Clean()
        {
            if (_dirtyDrawables)
                this.UpdateDrawables();

            if (_dirtyUpdatables)
                this.UpdateUpdatables();
        }

        public virtual void Update(GameTime gameTime)
        {
            this.Clean();

            foreach (TGameObject item in _updatables)
                item.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime)
        {
            foreach (TGameObject item in _drawables)
                item.Draw(gameTime);
        }

        private void UpdateDrawables()
        {
            _drawables = _list.Where(u => u.Visible)
                .OrderBy(u => u.DrawOrder)
                .ToArray();

            _dirtyDrawables = false;
        }

        private void UpdateUpdatables()
        {
            _updatables = _list.Where(u => u.Enabled)
                .OrderBy(u => u.UpdateOrder)
                .ToArray();

            _dirtyUpdatables = true;
        }

        protected override bool add(TGameObject item)
        {
            if(base.add(item))
            {
                item.DrawOrderChanged += this.HandleDrawOrderChanged;
                item.VisibleChanged += this.HandleVisibleChanged;
                item.UpdateOrderChanged += this.HandleUpdateOrderChanged;
                item.EnabledChanged += this.HandleEnabledChanged;

                if (item.Visible)
                    _dirtyDrawables = true;
                if (item.Enabled)
                    _dirtyUpdatables = true;

                return true;
            }

            return false;
        }

        protected override bool remove(TGameObject item)
        {
            if(base.remove(item))
            {
                item.DrawOrderChanged -= this.HandleDrawOrderChanged;
                item.VisibleChanged -= this.HandleVisibleChanged;
                item.UpdateOrderChanged -= this.HandleUpdateOrderChanged;
                item.EnabledChanged -= this.HandleEnabledChanged;

                return true;
            }

            return false;
        }

        private void HandleDrawOrderChanged(object sender, EventArgs e)
        {
            _dirtyDrawables = true;
        }

        private void HandleVisibleChanged(object sender, EventArgs e)
        {
            _dirtyDrawables = true;
        }

        private void HandleUpdateOrderChanged(object sender, EventArgs e)
        {
            _dirtyUpdatables = true;
        }

        private void HandleEnabledChanged(object sender, EventArgs e)
        {
            _dirtyUpdatables = true;
        }
    }
}
