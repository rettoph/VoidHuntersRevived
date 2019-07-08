using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Entities
{
    /// <summary>
    /// The pointer entity will act as the users
    /// doorway into the game. It, with the help
    /// of the mouse, allows users to interact
    /// with ship parts, aim their weapons, and
    /// more. 
    /// 
    /// The entity will be scoped and can be called via
    /// the servive provider.
    /// </summary>
    public class Pointer : Entity
    {
        private GraphicsDevice _graphics;
        private FarseerCamera2D _camera;
        private Vector3 _screenPosition;
        private Vector2 _newWorldPosition;
        private Vector2 _worldPosition;
        private Single _worldDelta;
        private Boolean _worldMoving;
        private Double _worldIdle;
        private Vector2 _newLocalPosition;
        private Vector2 _localPosition;
        private Single _localDelta;
        private Boolean _localMoving;
        private Double _localIdle;
        private Double _idleDelay;

        public Vector2 LocalPosition { get { return _localPosition; } }
        public Vector2 Position { get { return _worldPosition; } }
        public Boolean Primary { get; private set; }
        public Boolean Secondary { get; private set; }

        public event EventHandler<Vector2> OnPointerMovementStarted;
        public event EventHandler<Vector2> OnPointerMovementEnded;
        public event EventHandler<Vector2> OnPointerLocalMovementStarted;
        public event EventHandler<Vector2> OnPointerLocalMovementEnded;
        public event EventHandler<Boolean> OnPrimaryChanged;
        public event EventHandler<Boolean> OnSecondaryChanged;

        public Pointer(GraphicsDevice graphics, FarseerCamera2D camera, EntityConfiguration configuration, IServiceProvider provider) : base(configuration, provider)
        {
            _graphics = graphics;
            _camera = camera;
        }

        protected override void Boot()
        {
            base.Boot();

            _idleDelay = 250;
            _screenPosition = Vector3.Zero;
            _newWorldPosition = Vector2.Zero;
            _worldPosition = Vector2.Zero;
            _newLocalPosition = Vector2.Zero;
            _localPosition = Vector2.Zero;
        }

        protected override void update(GameTime gameTime)
        {
            // Local movement calculations
            if (_localDelta == 0 && _localMoving)
            {
                _localIdle += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (_localIdle >= _idleDelay)
                {
                    _localMoving = false;

                    this.OnPointerLocalMovementEnded?.Invoke(this, this.LocalPosition);
                }
            }
            else if (_localDelta > 0)
            {
                if (!_localMoving)
                {
                    _localMoving = true;

                    this.OnPointerLocalMovementStarted?.Invoke(this, this.LocalPosition);
                }

                _localIdle = 0;
            }

            // World movement calculations
            if (_worldDelta == 0 && _worldMoving)
            {
                _worldIdle += gameTime.ElapsedGameTime.TotalMilliseconds;

                if(_worldIdle >= _idleDelay)
                {
                    _worldMoving = false;

                    this.OnPointerMovementEnded?.Invoke(this, this.Position);
                }               
            }
            else if (_worldDelta > 0)
            {
                if (!_worldMoving)
                {
                    _worldMoving = true;

                    this.OnPointerMovementStarted?.Invoke(this, this.Position);
                }

                _worldIdle = 0;
            }
        }

        /// <summary>
        /// Input screen coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveTo(Single x, Single y)
        {
            _screenPosition.X = x;
            _screenPosition.Y = y;

            // Calculate the local position of the screen coordinates
            var lPos = _graphics.Viewport.Unproject(
                _screenPosition,
                Matrix.Identity,
                _camera.View,
                _camera.World);


            _newLocalPosition.X = lPos.X;
            _newLocalPosition.Y = lPos.Y;

            _localDelta = Vector2.Distance(_newLocalPosition, _localPosition);

            _localPosition.X = lPos.X;
            _localPosition.Y = lPos.Y;

            // Calculate the world position of the screen coordinates
            var wPos = _graphics.Viewport.Unproject(
                _screenPosition, 
                _camera.Projection, 
                _camera.View,
                _camera.World);


            _newWorldPosition.X = wPos.X;
            _newWorldPosition.Y = wPos.Y;

            _worldDelta = Vector2.Distance(_newWorldPosition, _worldPosition);

            _worldPosition.X = wPos.X;
            _worldPosition.Y = wPos.Y;
        }

        public void SetPrimary(Boolean value)
        {
            if(value != this.Primary)
            {
                this.Primary = value;
                this.OnPrimaryChanged?.Invoke(this, this.Primary);
            }
        }

        public void SetSecondary(Boolean value)
        {
            if (value != this.Secondary)
            {
                this.Secondary = value;
                this.OnSecondaryChanged?.Invoke(this, this.Secondary);
            }
        }
    }
}
