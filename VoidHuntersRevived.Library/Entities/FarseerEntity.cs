using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Configurations;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
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
        private Int16 _collisionGroup;

        private List<Fixture> _fixtureList;
        private Body _body;

        #region Protected Fields
        protected World world { get; private set; }
        #endregion

        #region Public Attributes
        public Int32 BodyId
        {
            get { return _body.BodyId; }
        }

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
        public Vector2 LocalCenter
        {
            get { return _body.LocalCenter; }
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
        public Int16 CollisionGroup
        {
            get { return _collisionGroup; }
            set
            {
                if (value != _collisionGroup)
                {
                    _collisionGroup = value;
                    _body.CollisionGroup = _collisionGroup;
                    this.OnCollisionGroupChanged?.Invoke(this, _collisionGroup);
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
        public event EventHandler<Int16> OnCollisionGroupChanged;
        public event EventHandler<Boolean> OnIsSensorChanged;
        public event EventHandler<Boolean> OnSleepingAllowedChanged;
        public event EventHandler<Boolean> OnPhysicsEnabledChanged;
        public event EventHandler<Body> OnBodyCreated;
        public event EventHandler<Fixture> OnFixtureCreated;
        public event EventHandler<Fixture> OnFixtureDestroyed;
        public event EventHandler<Vector2> OnLinearImpulseApplied;
        public event EventHandler<Single> OnAngularImpulseApplied;
        public event EventHandler<Body> OnSetTransform;
        #endregion

        #region Constructors
        public FarseerEntity(EntityConfiguration configuration, IServiceProvider provider) : base(configuration, provider)
        {
        }
        public FarseerEntity(Guid id, EntityConfiguration configuration, IServiceProvider provider) : base(id, configuration, provider)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            this.world = this.provider.GetRequiredService<World>();

            _collidesWith = Category.All;
            _collisionCategories = Category.Cat1;
            _fixtureList = new List<Fixture>();
            _body = this.CreateBody(this.world);

            this.Focused = new CounterBoolean();
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
        private Body CreateBody(
            World world, 
            Vector2 position = new Vector2(), 
            float rotation = 0)
        {
            var body = this.BuildBody(world, position, rotation);
            this.OnBodyCreated?.Invoke(this, body);
            return body;
        }

        protected virtual Body BuildBody(World world,
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
            fixture.CollisionGroup = this.CollisionGroup;
            fixture.IsSensor = this.IsSensor;

            this.OnFixtureCreated?.Invoke(this, fixture);

            return fixture;
        }

        public void DestroyFixture(Fixture fixture)
        {
            if (!_fixtureList.Contains(fixture))
                throw new Exception("Unable to destroy fixture, fixture unknown.");

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

        public void SetTransform(Vector2 position, Single rotation)
        {
            _body.SetTransform(position, rotation);

            this.OnSetTransform?.Invoke(this, _body);
        }

        protected internal Body GetBody()
        {
            return _body;
        }
        #endregion

        #region Network Methods
        protected override void read(NetIncomingMessage im)
        {
            if (im.ReadExists())
            { // Only read the body data if the server sent any
                this.Position = im.ReadVector2();
                this.Rotation = im.ReadSingle();
                this.LinearVelocity = im.ReadVector2();
                this.AngularVelocity = im.ReadSingle();
            }
        }

        protected override void write(NetOutgoingMessage om)
        {
            if(om.WriteExists(_body))
            { // Only write the body data if the body exists
                om.Write(this.Position);
                om.Write(this.Rotation);
                om.Write(this.LinearVelocity);
                om.Write(this.AngularVelocity);
            }
        }

        public void WritePositionData(NetOutgoingMessage om)
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
