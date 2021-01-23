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
    public class ShipPartShapeEditorService : ShipPartShapesServiceChildBase
    {
        #region Private Enums
        private enum EditFlags
        {
            None = 0,
            Editing = 1,
            Dragging = 2,
        }
        #endregion

        #region Private Fields
        private ShapeContext _shape;
        private EditFlags _flags;

        private Vector2 _dragStartMouseWorldPosition;
        private Vector2 _dragStartTranslation;

        private ShapeEditorMenu _menu;
        #endregion


        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.mouse.OnButtonStateChanged += this.HandleMouseButtonStateChanged;
        }

        protected override void PreRelease()
        {
            base.PreRelease();

            this.mouse.OnButtonStateChanged -= this.HandleMouseButtonStateChanged;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_flags.HasFlag(EditFlags.Dragging))
            { // We want to transform the current _editShape based on the mouse changes.
                _shape.Translation = _dragStartTranslation - _dragStartMouseWorldPosition + this.mouseWorldPosition;

                var editPoints = _shape.GetInterestPoints();
                var targetPoints = this.shapes.GetInterestPoints(_shape);
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
                    _shape.Translation += nearest.target - nearest.edit;
                }

                _menu.Transformations.Translation = _shape.Translation;
            }

            // Update the internal shape properties...
            if(_flags.HasFlag(EditFlags.Editing))
            {
                _shape.ClearSides();
                _shape.AddSides(_menu.SideContexts);

                _shape.Translation = _menu.Transformations.Translation;
                _shape.Rotation = _menu.Transformations.Rotation;
                _shape.Scale = _menu.Transformations.Scale;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (_flags.HasFlag(EditFlags.Editing))
            {
                _shape?.GetVertices().SkipLast(1).ForEach((v, i) =>
                {
                    _shape.Sides[i].Draw(
                        position: this.camera.Position + v,
                        worldRotation: _shape.GetWorldRotation(i),
                        primitiveBatch: this.primitiveBatch,
                        spriteBatch: this.spriteBatch,
                        font: this.font,
                        color: Color.White,
                        camera: this.camera,
                        scale: _shape.Scale);
                });
            }
        }
        #endregion

        #region API Methods
        public void Start(ShapeContext shape)
        {
            this.Stop();

            _shape = shape;
            _flags = EditFlags.Editing | EditFlags.Dragging;

            _dragStartMouseWorldPosition = this.mouseWorldPosition;
            _dragStartTranslation = _shape.Translation;

            _menu = this.shapes.Page.Menu.Children.Create<ShapeEditorMenu>((menu, p, c) =>
            {
                menu.shape = _shape;
            });
        }

        private void Stop()
        {
            _flags = EditFlags.None;
            _menu?.TryRelease();
        }
        #endregion

        #region Event Handlers
        private void HandleMouseButtonStateChanged(InputManager sender, InputArgs args)
        {
            this.synchronizer.Enqueue(gt =>
            {
                if (_flags.HasFlag(EditFlags.Editing))
                {
                    if (_flags.HasFlag(EditFlags.Dragging) || this.shapes.TestPointForShape(this.mouseWorldPosition) == _shape)
                    {
                        if(args.State == ButtonState.Pressed)
                        {
                            _flags |= EditFlags.Dragging;
                        }
                        else
                        {
                            _flags &= ~EditFlags.Dragging;
                        }
                    }
                    else if(!_menu.State.HasFlag(Guppy.UI.Enums.ElementState.Hovered))
                    {
                        this.Stop();
                    }
                }
            });
        }
        #endregion
    }
}
