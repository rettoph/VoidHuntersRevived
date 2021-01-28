using Guppy;
using Guppy.DependencyInjection;
using Guppy.IO.Input;
using Guppy.IO.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Builder.Contexts;
using VoidHuntersRevived.Builder.UI;
using Guppy.Extensions.System.Collections;
using Guppy.UI.Elements;
using Guppy.Events.Delegates;
using VoidHuntersRevived.Builder.Utilities;

namespace VoidHuntersRevived.Builder.Services
{
    /// <summary>
    /// Simple service specifically designed for 
    /// editing a pre-existing shape context.
    /// 
    /// Note, some of the original transformation 
    /// data will  be lost when importing 
    /// pre-existing shape files. This is unavoidable.
    /// </summary>
    public class ShipPartShapeEditorService : ShipPartShapesServiceEditorChildBase<ShapeContextBuilder>
    {
        #region Private Fields
        private ShapeEditorMenu _menu;
        #endregion

        #region Event Handlers
        public event OnEventDelegate<ShipPartShapeEditorService, ShapeContextBuilder> OnShapeDeleted;
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the internal shape properties...
            if(this.flags.HasFlag(EditFlags.Editing))
            {
                this.item.ClearSides();
                this.item.AddSides(_menu.SideContexts);

                this.item.Translation = _menu.Transformations.Translation;
                this.item.Rotation = _menu.Transformations.Rotation;
                this.item.Scale = _menu.Transformations.Scale;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (this.flags.HasFlag(EditFlags.Editing))
            {
                this.item?.GetVertices().SkipLast(1).ForEach((v, i) =>
                {
                    this.item.Sides[i].Draw(
                        position: this.camera.Position + v,
                        worldRotation: this.item.GetWorldRotation(i),
                        primitiveBatch: this.primitiveBatch,
                        spriteBatch: this.spriteBatch,
                        font: this.font,
                        color: Color.White,
                        camera: this.camera,
                        scale: this.item.Scale);
                });
            }
        }
        #endregion

        #region API Methods
        public override void Start(ShapeContextBuilder item)
        {
            base.Start(item);

            _menu = this.shapes.Page.Menu.Children.Create<ShapeEditorMenu>((menu, p, c) =>
            {
                menu.shape = item;
                menu.DeleteButton.OnClicked += this.HandleDeleteButtonClicked;
            });
        }

        public override void Stop()
        {
            base.Stop();

            if (_menu == default)
                return;

            _menu.DeleteButton.OnClicked -= this.HandleDeleteButtonClicked;
            _menu.TryRelease();
            _menu = null;
        }

        /// <inheritdoc />
        protected override void Drag(ShapeContextBuilder item, Vector2 position)
        {
            item.Translation = position;

            if(this.@lock[Enums.LockType.PointSnap])
            {
                var editPoints = item.GetInterestPoints();
                var targetPoints = this.shapes.GetInterestPoints(item);
                (Vector2 edit, Vector2 target, Single distance) nearest = (Vector2.Zero, Vector2.Zero, Single.MaxValue);

                foreach (Vector2 edit in editPoints)
                {
                    foreach (Vector2 target in targetPoints)
                    {
                        if (Vector2.Distance(edit, target) < nearest.distance)
                        {
                            nearest.edit = edit;
                            nearest.target = target;
                            nearest.distance = Vector2.Distance(edit, target);
                        }
                    }
                }

                if (nearest.distance < 0.25f)
                {
                    item.Translation += nearest.target - nearest.edit;
                }
            }

            _menu.Transformations.Translation = item.Translation;
        }

        protected override void Rotate(ShapeContextBuilder item, int count)
        {
            item.Rotation += MathHelper.ToRadians(5 * count);
            _menu.Transformations.Rotation = item.Rotation;
        }

        /// <inheritdoc />
        protected override Vector2 GetPosition(ShapeContextBuilder item)
            => item.Translation;

        /// <inheritdoc />
        protected override bool ItemUnder(ShapeContextBuilder item, Vector2 worldPosition)
            => this.shapes.TestPointForShape(this.mouseWorldPosition) == item;

        /// <inheritdoc />
        protected override bool ShouldStop()
            => !_menu.State.HasFlag(Guppy.UI.Enums.ElementState.Hovered);

        private void HandleDeleteButtonClicked(Element sender)
        {
            this.OnShapeDeleted?.Invoke(this, this.item);
            this.Stop();
        }
        #endregion
    }
}
