using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.Utilities;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Interfaces;
using Guppy.IO.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Builder.Enums;

namespace VoidHuntersRevived.Builder.Services
{
    public class ShipPartOuterHullBuilderService : ShipPartShapesServiceChildBase
    {
        #region Private Fields
        private Boolean _started;
        private List<Vector2> _vertices;
        #endregion

        #region Protected Properties
        protected override Vector2 mouseWorldPosition 
        {
            get
            {
                var pos = this.camera.Unproject(this.mouse.Position) - this.camera.Position;
                if (this.@lock[LockType.PointSnap])
                    return this.shapes.TryGetClosestInterestPoint(pos);
                else
                    return pos;
            }
        }
        #endregion

        #region Event Handlers
        public event OnEventDelegate<ShipPartOuterHullBuilderService, Vertices> OnOuterHullCompleted;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            _vertices = new List<Vector2>();

            this.commands["complete"].OnExcecute += this.HandleCompleteCommand;
        }

        protected override void Release()
        {
            base.Release();

            this.Stop();

            _vertices.Clear();

            _vertices = null;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (!_started)
                return;

            var cursor = this.mouseWorldPosition;
            var cursorScale = 0.25f;
            this.primitiveBatch.DrawLine(Color.Red, this.camera.Position + cursor - Vector2.UnitX * cursorScale, this.camera.Position + cursor + Vector2.UnitX * cursorScale);
            this.primitiveBatch.DrawLine(Color.Red, this.camera.Position + cursor - Vector2.UnitY * cursorScale, this.camera.Position + cursor + Vector2.UnitY * cursorScale);

            if (!_vertices.Any())
                return;

            Vector2 start = this.camera.Position + _vertices.First();
            Vector2 end;
            for(Int32 i=1; i<_vertices.Count; i++)
            {
                end = this.camera.Position + _vertices[i];

                this.primitiveBatch.DrawLine(Color.White, start, end);

                start = end;
            }

            this.primitiveBatch.DrawLine(Color.Gray, this.camera.Position + _vertices.Last(), this.camera.Position + this.mouseWorldPosition);


        }
        #endregion

        #region API Methods
        public void Start()
        {
            this.mouse.OnButtonStateChanged += this.HandleMouseButtonStateChanged;

            this.shapes.SetMessage(
                "Click anywhere in space to start hull.",
                $"{this.inputCommands["complete_outer_hull"].Input}: Complete Hull | {this.inputCommands["lock_point_snap"].Input}: Disable Snap");

            _started = true;
        }

        public void Stop()
        {
            this.OnOuterHullCompleted?.Invoke(this, new Vertices(_vertices.ToArray()));
            this.mouse.OnButtonStateChanged -= this.HandleMouseButtonStateChanged;
            _vertices.Clear();

            _started = false;
        }
        #endregion

        #region Event Handlers
        private void HandleMouseButtonStateChanged(InputManager sender, InputArgs args)
        {
            if (!_started || args.State == ButtonState.Pressed)
                return; // Nothing should happen...

            // Save the added vertice...
            switch (args.Which.MouseButton)
            {
                case Guppy.IO.Enums.MouseButton.Left:
                    _vertices.Add(this.mouseWorldPosition);
                    break;
                case Guppy.IO.Enums.MouseButton.Right:
                    if (_vertices.Any())
                        _vertices.Remove(_vertices.Last());
                    else
                        this.Stop();
                    break;
            }
        }

        private CommandResponse HandleCompleteCommand(ICommand sender, CommandInput input)
        {
            if (input.GetIfContains<CompleteType>("type") != CompleteType.OuterHull || !_started)
                return CommandResponse.Empty; // This invocation is not relevant... just ignore it.

            this.Stop();

            return CommandResponse.Success();
        }
        #endregion
    }
}
