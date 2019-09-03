using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GalacticFighters.Library.CustomEventArgs;
using GalacticFighters.Library.Scenes;
using GalacticFighters.Library.Utilities;
using Guppy.Configurations;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities
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
                    this.Events.TryInvoke(this, "collides-with:changed", _collidesWith);
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
                    this.Events.TryInvoke(this, "collision-categories:changed", _collisionCategories);
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
                    this.Events.TryInvoke(this, "collision-group:changed", _collisionGroup);
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
                    this.Events.TryInvoke(this, "is-sensor:changed", _isSensor);
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
                    this.Events.TryInvoke(this, "sleeping-allowed:changed", _body.SleepingAllowed);
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
                    this.Events.TryInvoke(this, "physics-enabled:changed", _body.Enabled);
                }
            }
        }
        public Single AngularDamping
        {
            get
            {
                return _body.AngularDamping;
            }
            set
            {
                if (value != _body.AngularDamping)
                {
                    _body.AngularDamping = value;
                    this.Events.TryInvoke(this, "angular-damping:changed", _body.AngularDamping);
                }
            }
        }
        public Single LinearDamping
        {
            get
            {
                return _body.LinearDamping;
            }
            set
            {
                if (value != _body.LinearDamping)
                {
                    _body.LinearDamping = value;
                    this.Events.TryInvoke(this, "linear-damping:changed", _body.LinearDamping);
                }
            }
        }
        public CounterBoolean Focused { get; private set; }
        #endregion

        #region Constructors

        #endregion

        #region Initialization Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            // Register required events...
            this.Events.Register<Category>("collides-with:changed");
            this.Events.Register<Category>("collision-categories:changed");
            this.Events.Register<Int16>("collision-group:changed");
            this.Events.Register<Boolean>("is-sensor:changed");
            this.Events.Register<Boolean>("sleeping-allowed:changed");
            this.Events.Register<Boolean>("physics-enabled:changed");
            this.Events.Register<Single>("angular-damping:changed");
            this.Events.Register<Single>("linear-damping:changed");

            this.Events.Register<Body>("body:created");
            this.Events.Register<Fixture>("fixture:created");
            this.Events.Register<Fixture>("fixture:destroyed");
            this.Events.Register<Vector2>("linear-impulse:applied");
            this.Events.Register<Single>("angular-impulse:applied");
            this.Events.Register<ForceEventArgs>("force:applied");
            this.Events.Register<Body>("transform:set");
        }

        protected override void Initialize()
        {
            base.Initialize();

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
            this.Events.TryInvoke(this, "body:created", body);
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

            this.Events.TryInvoke(this, "fixture:created", fixture);

            return fixture;
        }

        public void DestroyFixture(Fixture fixture)
        {
            if (!_fixtureList.Contains(fixture))
                throw new Exception("Unable to destroy fixture, fixture unknown.");

            _body.DestroyFixture(fixture);
            _fixtureList.Remove(fixture);

            this.Events.TryInvoke(this, "fixture:destroyed", fixture);
        }

        public void ApplyLinearImpulse(Vector2 impulse)
        {
            _body.ApplyLinearImpulse(impulse);

            this.Events.TryInvoke(this, "linear-impulse:applied", impulse);
        }

        public void ApplyAngularImpulse(Single impulse)
        {
            _body.ApplyAngularImpulse(impulse);

            this.Events.TryInvoke(this, "angular-impulse:applied", impulse);
        }

        public void ApplyForce(Vector2 force, Vector2 point)
        {
            _body.ApplyForce(force, point);

            this.Events.TryInvoke(this, "force:applied", new ForceEventArgs(force, point));
        }

        public void SetTransform(Vector2 position, Single rotation)
        {
            _body.SetTransform(position, rotation);

            this.Events.TryInvoke(this, "transform:set", _body);
        }

        protected internal Body GetBody()
        {
            return _body;
        }
        #endregion

        #region Network Methods
        protected override void Read(NetIncomingMessage im)
        {
            if (im.ReadExists())
            { // Only read the body data if the server sent any
                this.Position = im.ReadVector2();
                this.Rotation = im.ReadSingle();
                this.LinearVelocity = im.ReadVector2();
                this.AngularVelocity = im.ReadSingle();
            }
        }

        protected override void Write(NetOutgoingMessage om)
        {
            if (om.WriteExists(_body))
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
