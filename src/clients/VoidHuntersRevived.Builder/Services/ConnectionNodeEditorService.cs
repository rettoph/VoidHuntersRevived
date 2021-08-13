using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts;
using Guppy.Extensions.Utilities;
using VoidHuntersRevived.Builder.UI;
using Guppy.Events.Delegates;

namespace VoidHuntersRevived.Builder.Services
{
    /// <summary>
    /// Simple helper class useful for editing connection nodes.
    /// </summary>
    public class ConnectionNodeEditorService : ShipPartShapesServiceEditorChildBase<ConnectionNodeContext>
    {
        #region Private Fields
        private ConnectionNodeEditorMenu _menu;
        #endregion

        #region Events
        public OnEventDelegate<ConnectionNodeEditorService, ConnectionNodeContext> OnConnectionNodeDeleted;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.flags.HasFlag(EditFlags.Editing))
            {
                if (this.flags.HasFlag(EditFlags.Dragging))
                    _menu.Position = this.item.Position;

                this.item.Position = _menu.Position;
                this.item.Rotation = _menu.Rotation;
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if(this.flags.HasFlag(EditFlags.Editing))
                this.primitiveBatch.TraceCircle(
                    Color.Cyan,
                    this.camera.Position + this.item.Position,
                    0.1f);
        }
        #endregion

        #region Helper Methods
        public void Start(ConnectionNodeContext item, Boolean deleteable)
        {
            base.Start(item);

            _menu = this.shapes.Page.Menu.Children.Create<ConnectionNodeEditorMenu>((menu, p, c) =>
            {
                menu.deleteable = deleteable;
                menu.OnDelete += this.HandleDelete;
            });

            _menu.Position = item.Position;
            _menu.Rotation = item.Rotation;
        }

        public override void Start(ConnectionNodeContext item)
        {
            this.Start(item, false);
        }

        public override void Stop()
        {
            base.Stop();

            if (_menu == default)
                return;

            _menu.OnDelete -= this.HandleDelete;
            _menu.TryRelease();
            _menu = null;
        }

        protected override void Drag(ConnectionNodeContext item, Vector2 position)
        {
            item.Position = position;

            if (this.@lock[Enums.LockType.PointSnap])
            {
                var interestPoints = this.shapes.GetInterestPoints();

                foreach (Vector2 point in interestPoints)
                {
                    if (point != item.Position && Vector2.Distance(point, item.Position) < 0.25f)
                    {
                        item.Position = point;
                        break;
                    }
                }
            }
        }

        protected override void Rotate(ConnectionNodeContext item, int count)
        {
            item.Rotation += MathHelper.ToRadians(5 * count);
            _menu.Rotation = item.Rotation;
        }

        protected override Vector2 GetPosition(ConnectionNodeContext item)
            => item.Position;

        protected override bool ItemUnder(ConnectionNodeContext item, Vector2 worldPosition)
            => Vector2.Distance(item.Position, worldPosition) < 0.1f;

        protected override bool ShouldStop()
            => !_menu.State.HasFlag(Guppy.UI.Enums.ElementState.Hovered);
        #endregion

        #region Event Handlers
        private void HandleDelete(ConnectionNodeEditorMenu sender)
        {
            this.OnConnectionNodeDeleted?.Invoke(this, this.item);
            this.Stop();
        }
        #endregion
    }
}
