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

        private Int32 _scroll;
        private Int32 _scrollDelta;
        private Double _scrollIdle;
        private Boolean _scrollMoving;

        public Vector2 LocalPosition { get { return _localPosition; } }
        public Vector2 Position { get { return _worldPosition; } }
        public Boolean Primary { get; private set; }
        public Boolean Secondary { get; private set; }
        public Int32 Scroll { get { return _scroll; } }
        public Int32 ScrollDelta { get { return _scrollDelta; } }

        public event EventHandler<Int32> OnScrollStarted;
        public event EventHandler<Int32> OnScrollEnded;
        public event EventHandler<Int32> OnScrolled;
        public event EventHandler<Vector2> OnMovementStarted;
        public event EventHandler<Vector2> OnMovementEnded;
        public event EventHandler<Vector2> OnLocalMovementStarted;
        public event EventHandler<Vector2> OnLocalMovementEnded;
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

                    this.OnLocalMovementEnded?.Invoke(this, this.LocalPosition);
                }
            }
            else if (_localDelta > 0)
            {
                if (!_localMoving)
                {
                    _localMoving = true;

                    this.OnLocalMovementStarted?.Invoke(this, this.LocalPosition);
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

                    this.OnMovementEnded?.Invoke(this, this.Position);
                }               
            }
            else if (_worldDelta > 0)
            {
                if (!_worldMoving)
                {
                    _worldMoving = true;

                    this.OnMovementStarted?.Invoke(this, this.Position);
                }

                _worldIdle = 0;
            }

            // Scroll calculations
            if (_scrollDelta == 0 && _scrollMoving)
            {
                _scrollIdle += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (_scrollIdle >= _idleDelay)
                {
                    _scrollMoving = false;

                    this.OnScrollEnded?.Invoke(this, _scroll);
                }
            }
            else if (_scrollDelta != 0)
            {
                if(!_scrollMoving)
                {
                    _scrollMoving = true;

                    this.OnScrollStarted?.Invoke(this, _scroll);
                }

                this.OnScrolled?.Invoke(this, _scrollDelta);

                _scrollIdle = 0;
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

        public void ScrollTo(Int32 value)
        {
            _scrollDelta = value - _scroll;
            _scroll = value;
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
