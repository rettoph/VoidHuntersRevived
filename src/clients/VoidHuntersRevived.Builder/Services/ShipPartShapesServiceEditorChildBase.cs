using Guppy.DependencyInjection;
using Guppy.IO.Input;
using Guppy.IO.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Builder.Services
{
    /// <summary>
    /// A simple class used to "edit" something.
    /// Generally useful for positional updates
    /// and opening a menu dialog.
    /// 
    /// This is used to drag/drop both connection nodes
    /// & existing shapes.
    /// </summary>
    public abstract class ShipPartShapesServiceEditorChildBase<T> : ShipPartShapesServiceChildBase
    {
        #region Private Enums
        public enum EditFlags
        {
            None = 0,
            Editing = 1,
            Dragging = 2,
        }
        #endregion

        #region Private Fields
        private EditFlags _flags;

        private Vector2 _dragStartMouseWorldPosition;
        private Vector2 _dragStartPosition;
        #endregion

        #region Protected Properties
        protected EditFlags flags => _flags;

        protected T item { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.mouse.OnButtonStateChanged += this.HandleMouseButtonStateChanged;
            this.mouse.OnScrollWheelValueChanged += this.HandleScrollWheelValueChanged;
        }

        protected override void PreRelease()
        {
            base.PreRelease();

            this.mouse.OnButtonStateChanged -= this.HandleMouseButtonStateChanged;
            this.mouse.OnScrollWheelValueChanged -= this.HandleScrollWheelValueChanged;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_flags.HasFlag(EditFlags.Dragging))
                this.Drag(this.item, _dragStartPosition - _dragStartMouseWorldPosition + this.mouseWorldPosition);
        }
        #endregion

        #region Helper Methods
        public virtual void Start(T item)
        {
            this.Stop();

            this.item = item;
            _flags = EditFlags.Editing | EditFlags.Dragging;

            _dragStartMouseWorldPosition = this.mouseWorldPosition;
            _dragStartPosition = this.GetPosition(item);
        }

        public virtual void Stop()
        {
            _flags = EditFlags.None;
        }

        /// <summary>
        /// Attempt to move the current item by the recieved delta amount.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="position"></param>
        protected abstract void Drag(T item, Vector2 position);

        /// <summary>
        /// Attempt to rotate the current item n number of
        /// "blocks"
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        protected abstract void Rotate(T item, Int32 count);

        /// <summary>
        /// Get the recieved item's position.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract Vector2 GetPosition(T item);

        /// <summary>
        /// Determin if the item is "under" the recieved coordinates.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        protected abstract Boolean ItemUnder(T item, Vector2 worldPosition);

        /// <summary>
        /// Determin if the current service should be stopped.
        /// This is generally in response to <see cref="ItemUnder(T, Vector2)"/>
        /// being false and the user not interacting with any UI elements.
        /// 
        /// Think if the user clicking on another item or in general space.
        /// </summary>
        /// <returns></returns>
        protected abstract Boolean ShouldStop();
        #endregion

        #region Event Handlers
        private void HandleMouseButtonStateChanged(InputManager sender, InputArgs args)
        {
            this.synchronizer.Enqueue(gt =>
            {
                if (_flags.HasFlag(EditFlags.Editing))
                {
                    if (_flags.HasFlag(EditFlags.Dragging) || this.ItemUnder(this.item, this.mouseWorldPosition))
                    {
                        if (args.State == ButtonState.Pressed)
                        {
                            _flags |= EditFlags.Dragging;
                        }
                        else
                        {
                            _flags &= ~EditFlags.Dragging;
                        }
                    }
                    else if (this.ShouldStop())
                    {
                        this.Stop();
                    }
                }
            });
        }

        private void HandleScrollWheelValueChanged(MouseService sender, ScrollWheelArgs args)
        {
            this.synchronizer.Enqueue(gt =>
            {
                if (_flags.HasFlag(EditFlags.Editing))
                {
                    this.Rotate(this.item, (Int32)(args.Delta / 120));
                }
            });
        }
        #endregion
    }
}
