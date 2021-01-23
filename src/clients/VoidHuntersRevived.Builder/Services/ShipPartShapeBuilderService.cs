using Guppy;
using Guppy.DependencyInjection;
using Guppy.IO.Enums;
using Guppy.IO.Input;
using Guppy.IO.Services;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using VoidHuntersRevived.Builder.Contexts;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Extensions.Microsoft.Xna.Framework.Graphics;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.System.Collections;
using System.Linq;
using Guppy.Extensions.Utilities;
using Microsoft.Xna.Framework.Input;
using Guppy.IO.Input.Services;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Contexts;
using tainicom.Aether.Physics2D.Common;
using Guppy.Events.Delegates;
using VoidHuntersRevived.Builder.Enums;

namespace VoidHuntersRevived.Builder.Services
{
    /// <summary>
    /// A simple helper class used to manage the construction of a single ship.
    /// </summary>
    public class ShipPartShapeBuilderService : ShipPartShapesServiceChildBase
    {
        #region Private Fields
        private ShapeContext _shape;
        private Boolean _building;
        private Vector2 _origin;

        private Vector2? _start;
        #endregion

        #region Protected Properties
        protected override Vector2 mouseWorldPosition => this.camera.Unproject(this.mouse.Position) - _origin;
        private Vector2 lastVertex => _shape.GetVertices().Last();
        #endregion

        #region Public Properties
        public ShapeContext Shape => _shape;
        #endregion

        #region Events
        public event OnEventDelegate<ShipPartShapeBuilderService, ShapeContext> OnShapeBuilt;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.mouse.OnButtonStateChanged += this.HandleMouseButtonStateChanged;
        }

        protected override void Release()
        {
            this.mouse.OnButtonStateChanged -= this.HandleMouseButtonStateChanged;

            base.Release();
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if(_building)
            {
                _shape?.GetVertices().SkipLast(1).ForEach((v, i) =>
                {
                    _shape.Sides[i].Draw(
                        position: _origin + v,
                        worldRotation: _shape.GetWorldRotation(i),
                        primitiveBatch: this.primitiveBatch,
                        spriteBatch: this.spriteBatch,
                        font: this.font,
                        color: Color.White,
                        camera: this.camera);
                });

                if(_start != default)
                    this.CalculateNextSideContext().Draw(
                        position: _origin + this.lastVertex,
                        worldRotation: _shape?.LastWorldRotation ?? 0,
                        primitiveBatch: this.primitiveBatch,
                        spriteBatch: this.spriteBatch,
                        font: this.font,
                        color: Color.Gray,
                        camera: this.camera);

                var cursor = this.@lock[LockType.PointSnap] ? this.shapes.TryGetClosestInterestPoint(this.mouseWorldPosition) : this.mouseWorldPosition;
                var cursorScale = 0.25f;
                this.primitiveBatch.DrawLine(Color.Red, _origin + cursor - Vector2.UnitX * cursorScale, _origin + cursor + Vector2.UnitX * cursorScale);
                this.primitiveBatch.DrawLine(Color.Red, _origin + cursor - Vector2.UnitY * cursorScale, _origin + cursor + Vector2.UnitY * cursorScale);
            }

        }
        #endregion

        #region API Methods
        /// <summary>
        /// Begin tracking user input and build
        /// a shape based on it.
        /// </summary>
        /// <param name="origin"></param>
        public void Start(Vector2 origin)
        {
            _origin = origin;
            _building = true;
            _shape = new ShapeContext();
        }

        /// <summary>
        /// Mark the internal shape as complete.
        /// </summary>
        public void End()
        {
            this.OnShapeBuilt?.Invoke(this, _shape);

            _building = false;
            _shape = null;
            _start = null;
        }

        /// <summary>
        /// Conver tthe recieved vertex and convert into a valid
        /// <see cref="SideContext"/> if possible.
        /// </summary>
        private void AddVertex()
        {
            if(_start == default)
            { // No starting location has been defined yet...

                // Save the current shape's start location!
                if (this.@lock[LockType.PointSnap])
                    _start = this.shapes.TryGetClosestInterestPoint(this.mouseWorldPosition);
                else
                    _start = this.mouseWorldPosition;

                // Update the default translation values.
                _shape.Translation = _start.Value;
            }
            else if (!_shape.TryAddSide(this.CalculateNextSideContext()))
            { // We tried adding a side but it failed?
                if(_shape.Sides.Count > 1)
                { // The shapes valid, just finish it i guess hehe
                    this.End();
                }
            }
        }

        /// <summary>
        /// Remove the last pertect added into the shape.
        /// </summary>
        private void RemoveVertex()
        {
            if (_shape.TryRemoveSide())
                return;

            _start = null;
        }

        /// <summary>
        /// Calculate the next side context to be added based
        /// on the <see cref="_lastVertex"/> & the <see cref="mouseWorldPosition"/>
        /// values. This does not actually add the side into anything.
        /// </summary>
        /// <returns></returns>
        private SideContext CalculateNextSideContext()
        {
            if (_start == default)
                return default;

            var target = this.mouseWorldPosition;
            if (this.@lock[LockType.PointSnap])
                target = this.shapes.TryGetClosestInterestPoint(target);

            var lengthInterval = 1f;
            var length = Vector2.Distance(this.lastVertex, target);

            var rotationInterval = MathHelper.ToRadians(5);
            var rotation = -1 * MathHelper.WrapAngle(target.Angle(this.lastVertex) - (_shape.LastWorldRotation + MathHelper.Pi));
            rotation = rotation < -MathHelper.PiOver2 ? MathHelper.Pi : rotation;
            rotation = rotation < 0 ? 0 : rotation;

            if(this.@lock[LockType.Length])
                length = (Single)Math.Max(1, Math.Round(length / lengthInterval)) * lengthInterval;
            if(this.@lock[LockType.Rotation])
                rotation = (Single)Math.Round(rotation / rotationInterval) * rotationInterval;

            return new SideContext()
            {
                Length = length,
                Rotation = rotation
            };
        }
        #endregion

        #region Event Handlers
        private void HandleMouseButtonStateChanged(InputManager sender, InputArgs args)
        {
            if (args.State == ButtonState.Released)
                return;

            this.synchronizer.Enqueue(gt =>
            {
                if (_building)
                { // Only bother listening to mouse inouts if the system is building a shape anyway...
                    switch (args.Which.CursorButton)
                    {
                        case MouseButton.Left:
                            this.AddVertex();
                            break;
                        case MouseButton.Middle:
                            break;
                        case MouseButton.Right:
                            this.RemoveVertex();
                            break;
                    }
                }
            });
        }
        #endregion
    }
}
