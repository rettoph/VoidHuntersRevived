using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Configurations;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Entity representing an object within the farseer world.
    /// </summary>
    public partial class FarseerEntity : NetworkEntity
    {
        private Boolean _isSensor;
        private Category _collidesWith;
        private Category _collisionCategories;

        private List<Fixture> _fixtureList;
        private Body _body;

        #region Public Attributes
        public Vector2 Position
        {
            get { return _body.Position; }
            set { _body.Position = value; }
        }
        public Single Rotation
        {
            get { return _body.Rotation; }
            set { _body.Rotation = value; }
        }
        public Vector2 LinearVelocity
        {
            get { return _body.LinearVelocity; }
            set { _body.LinearVelocity = value; }
        }
        public Single AngularVelocity
        {
            get { return _body.AngularVelocity; }
            set { _body.AngularVelocity = value; }
        }
        public Vector2 WorldCenter
        {
            get { return _body.WorldCenter; }
        }
        public Boolean Awake
        {
            get { return _body.Awake; }
            set { _body.Awake = value; }
        }
        public Category CollidesWith
        {
            get { return _collidesWith; }
            set
            {
                if (value != _collidesWith)
                {
                    _collidesWith = value;
                    _body.CollidesWith = _collidesWith;
                    this.OnCollidesWithChanged?.Invoke(this, _collidesWith);
                }
            }
        }
        public Category CollisionCategories
        {
            get { return _collisionCategories; }
            set
            {
                if (value != _collisionCategories)
                {
                    _collisionCategories = value;
                    _body.CollisionCategories = _collisionCategories;
                    this.OnCollisionCategoriesChanged?.Invoke(this, _collisionCategories);
                }
            }
        }
        public Boolean IsSensor
        {
            get { return _isSensor; }
            set
            {
                if (value != _isSensor)
                {
                    _isSensor = value;
                    _body.IsSensor = value;
                    this.OnIsSensorChanged?.Invoke(this, _isSensor);
                }
            }
        }
        public Boolean SleepingAllowed
        {
            get { return _body.SleepingAllowed; }
            set
            {
                if (value != _body.SleepingAllowed)
                {
                    _body.SleepingAllowed = value;
                    this.OnSleepingAllowedChanged?.Invoke(this, _body.SleepingAllowed);
                }
            }
        }
        public Boolean PhysicsEnabled
        {
            get { return _body.Enabled; }
            set
            {
                if (value != _body.Enabled)
                {
                    _body.Enabled = value;
                    this.OnPhysicsEnabledChanged?.Invoke(this, _body.Enabled);
                }
            }
        }
        public CounterBoolean Focused { get; private set; }
        #endregion

        #region Events
        public event EventHandler<Category> OnCollidesWithChanged;
        public event EventHandler<Category> OnCollisionCategoriesChanged;
        public event EventHandler<Boolean> OnIsSensorChanged;
        public event EventHandler<Boolean> OnSleepingAllowedChanged;
        public event EventHandler<Boolean> OnPhysicsEnabledChanged;
        public event EventHandler<Fixture> OnFixtureCreated;
        public event EventHandler<Fixture> OnFixtureDestroyed;
        public event EventHandler<Vector2> OnLinearImpulseApplied;
        public event EventHandler<Single> OnAngularImpulseApplied;
        #endregion

        #region Constructors
        public FarseerEntity(EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(configuration, scene, provider, logger)
        {
        }
        public FarseerEntity(Guid id, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(id, configuration, scene, provider, logger)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            _collidesWith = Category.All;
            _collisionCategories = Category.Cat1;
            _fixtureList = new List<Fixture>();

            this.Focused = new CounterBoolean();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            this._body = this.CreateBody((this.scene as VoidHuntersWorldScene).World);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
        #endregion

        #region Farseer Methods
        /// <summary>
        /// Create a new body within a given world.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="bodyType"></param>
        /// <returns></returns>
        public virtual Body CreateBody(
            World world, 
            Vector2 position = new Vector2(), 
            float rotation = 0)
        {
            var body = BodyFactory.CreateBody(
                world,
                position,
                rotation,
                BodyType.Dynamic,
                this);

            body.LinearDamping = 1f;
            body.AngularDamping = 1f;

            return body;
        }

        public Fixture CreateFixture(Shape shape, Object userData = null)
        {
            var fixture = _body.CreateFixture(shape, userData);
            _fixtureList.Add(fixture);

            // Update the fixture collision categories
            fixture.CollidesWith = this.CollidesWith;
            fixture.CollisionCategories = this.CollisionCategories;
            fixture.IsSensor = this.IsSensor;

            this.OnFixtureCreated?.Invoke(this, fixture);

            return fixture;
        }

        public void DestroyFixture(Fixture fixture)
        {
            if (!_fixtureList.Contains(fixture))
                throw new Exception("Unable to destroy fixture, shape unknown.");

            _body.DestroyFixture(fixture);
            _fixtureList.Remove(fixture);

            this.OnFixtureDestroyed?.Invoke(this, fixture);
        }

        public void ApplyLinearImpulse(Vector2 impulse)
        {
            _body.ApplyLinearImpulse(impulse);

            this.OnLinearImpulseApplied?.Invoke(this, impulse);
        }

        public void ApplyAngularImpulse(Single impulse)
        {
            _body.ApplyAngularImpulse(impulse);

            this.OnAngularImpulseApplied?.Invoke(this, impulse);
        }
        #endregion

        #region Network Methods
        protected override void read(NetIncomingMessage im)
        {
            this.Position = im.ReadVector2();
            this.Rotation = im.ReadSingle();
            this.LinearVelocity = im.ReadVector2();
            this.AngularVelocity = im.ReadSingle();
        }

        protected override void write(NetOutgoingMessage om)
        {
            om.Write(this.Position);
            om.Write(this.Rotation);
            om.Write(this.LinearVelocity);
            om.Write(this.AngularVelocity);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            _body.Dispose();
        }

        #region Operator Overrides
        public static implicit operator FarseerEntity(Body body)
        {
            return body.UserData as FarseerEntity;
        }
        #endregion
    }
}
