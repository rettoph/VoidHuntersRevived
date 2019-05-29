using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Scenes;
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
        private Vector2 _newPosition;
        private Vector2 _position;
        private Single _delta;
        private Boolean _moving;
        private Double _idleDelay;
        private Double _idle;
        private Body _body;
        private Fixture _fixture;
        private List<FarseerEntity> _contacts;

        public IReadOnlyCollection<FarseerEntity> Contacts
        {
            get { return _contacts.AsReadOnly(); }
        }
        public Vector2 Position { get { return _position; } }
        public Boolean Primary { get; private set; }
        public Boolean Secondary { get; private set; }

        public event EventHandler<Vector2> OnPointerMovementStarted;
        public event EventHandler<Vector2> OnPointerMovementEnded;
        public event EventHandler<FarseerEntity> OnContactStarted;
        public event EventHandler<FarseerEntity> OnContactEnded;
        public event EventHandler<Boolean> OnPrimaryChanged;
        public event EventHandler<Boolean> OnSecondaryChanged;

        public Pointer(EntityConfiguration configuration, VoidHuntersClientWorldScene scene, IServiceProvider provider, ILogger logger) : base(configuration, scene, provider, logger)
        {
            _body = BodyFactory.CreateBody(
                scene.World,
                Vector2.Zero,
                0,
                BodyType.Dynamic);
            _body.SleepingAllowed = false;

            _fixture = FixtureFactory.AttachCircle(1f, 0.0f, _body);
            _fixture.IsSensor = true;
            _fixture.CollidesWith = Category.All;

            scene.World.ContactManager.BeginContact += this.HandleBeginContact;
            scene.World.ContactManager.EndContact += this.HandleEndContact;
        }

        protected override void Boot()
        {
            base.Boot();

            _idleDelay = 250;
            _newPosition = Vector2.Zero;
            _position = Vector2.Zero;
            _contacts = new List<FarseerEntity>();


        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_delta == 0 && _moving)
            {
                _idle += gameTime.ElapsedGameTime.TotalMilliseconds;

                if(_idle >= _idleDelay)
                {
                    _moving = false;

                    this.OnPointerMovementEnded?.Invoke(this, this.Position);
                }               
            }
            else if (_delta > 0)
            {
                if (!_moving)
                {
                    _moving = true;

                    this.OnPointerMovementStarted?.Invoke(this, this.Position);
                }

                _idle = 0;
            }

            _body.Position = this.Position;
        }

        public void MoveTo(Single x, Single y)
        {
            _newPosition.X = x;
            _newPosition.Y = y;

            _delta = Vector2.Distance(_newPosition, _position);

            _position.X = x;
            _position.Y = y;
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

        private void StartContact(FarseerEntity target)
        {
            if (target != null)
            {
                _contacts.Add(target);

                this.OnContactStarted?.Invoke(this, target);
            }
        }

        private void EndContact(FarseerEntity target)
        {
            if (target != null)
            {
                _contacts.Remove(target);

                this.OnContactEnded?.Invoke(this, target);
            }
        }

        #region Event Handlers
        private bool HandleBeginContact(Contact contact)
        {
            if (contact.FixtureA == _fixture)
                this.StartContact(contact.FixtureB.Body.UserData as FarseerEntity);
            if (contact.FixtureB == _fixture)
                this.StartContact(contact.FixtureA.Body.UserData as FarseerEntity);

            return true;
        }

        private void HandleEndContact(Contact contact)
        {
            if (contact.FixtureA == _fixture)
                this.EndContact(contact.FixtureB.Body.UserData as FarseerEntity);
            if (contact.FixtureB == _fixture)
                this.EndContact(contact.FixtureA.Body.UserData as FarseerEntity);
        }
        #endregion
    }
}
