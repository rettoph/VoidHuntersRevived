using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using VoidHuntersRevived.Library.Extensions.Farseer;
using VoidHuntersRevived.Library.Structs;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Pooling.Interfaces;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// An Entity that contains a body
    /// and can hold fixtures.
    /// </summary>
    public abstract class FarseerEntity : NetworkEntity
    {
        #region Private Attributes
        /// <summary>
        /// The raw farseer world the entity resides in.
        /// </summary>
        private World _world;

        private IPool<AppliedForce> _forcePool;
        #endregion

        #region Protected Fields
        /// <summary>
        /// The raw body managed by this FarseerEntity. It is not recommened that
        /// you interact with the body derectly, but use the interface available to
        /// you within the FarseerEntity.
        /// </summary>
        protected Body body { get; private set; }
        #endregion

        #region Public Attributes
        /// <summary>
        /// The internal Farseer Boyy's Id.
        /// </summary>
        public Int32 BodyId { get => this.body.BodyId; }

        /// <summary>
        /// Reservations can be made onto ShipParts.
        /// A shipPart with no reservations will automatically
        /// disable itself when the body falls asleep. If the ShipPart
        /// has a reservation, however, it will keep itself enabled.
        /// </summary>
        public CounterBoolean Reserverd { get; private set; }

        /// <summary>
        /// Get the sleep state of the body. A sleeping body has very
        /// low CPU cost.
        /// </summary>
        public Boolean Awake { get { return this.body.Awake; } }

        /// <summary>
        /// Get the world body origin position.
        /// </summary>
        public virtual Vector2 Position { get { return this.body.Position; } }

        /// <summary>
        /// Get the angle in radians.
        /// </summary>
        public virtual Single Rotation { get { return this.body.Rotation; } }

        /// <summary>
        /// Get the linear velocity of the center of mass.
        /// </summary>
        public virtual Vector2 LinearVelocity { get { return this.body.LinearVelocity; } }

        /// <summary>
        /// Gets the angular velocity. Radians/second.
        /// </summary>
        public virtual Single AngularVelocity { get { return this.body.AngularVelocity; } }

        /// <summary>
        /// Get the world position of the center of mass.
        /// </summary>
        /// <value>The world position.</value>
        public virtual Vector2 WorldCenter { get { return this.body.WorldCenter; } }

        /// <summary>
        /// Get the local position of the center of mass.
        /// </summary>
        /// <value>The local position.</value>
        public virtual Vector2 LocalCenter { get { return this.body.LocalCenter; } }

        /// <summary>
        /// Tthe active state of the body. An inactive body is not
        /// simulated and cannot be collided with or woken up.
        /// If you pass a flag of true, all fixtures will be added to the
        /// broad-phase.
        /// If you pass a flag of false, all fixtures will be removed from
        /// the broad-phase and all contacts will be destroyed.
        /// Fixtures and joints are otherwise unaffected. You may continue
        /// to create/destroy fixtures and joints on inactive bodies.
        /// Fixtures on an inactive body are implicitly inactive and will
        /// not participate in collisions, ray-casts, or queries.
        /// Joints connected to an inactive body are implicitly inactive.
        /// An inactive body is still owned by a b2World object and remains
        /// in the body list.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public Boolean BodyEnabled { get { return this.body.Enabled; } }

        public Category CollidesWith { get;  private set; }
        public Category CollisionCategories { get; private set; }
        public Category IgnoreCCDWith { get; private set; }

        public Int32 FixtureCount { get => this.body.FixtureList.Count; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _world = provider.GetRequiredService<World>();
            _forcePool = provider.GetRequiredService<IPool<AppliedForce>>();

            // Automatically enable the ShipPart when a reservation is made.
            this.Reserverd = new CounterBoolean(value => {
                this.Events.TryInvoke<Boolean>(this, "reserved:changed", value);
            });

            // Initialize basic events
            this.Events.Register<Body>("body:created");
            this.Events.Register<Body>("body:destroyed");
            this.Events.Register<Fixture>("fixture:created");
            this.Events.Register<Fixture>("fixture:destroyed");

            this.Events.Register<Boolean>("body-enabled:changed");
            this.Events.Register<Body>("position:changed");
            this.Events.Register<Body>("velocity:changed");
            this.Events.Register<Vector2>("linear-impulse:applied");
            this.Events.Register<Single>("angular-impulse:applied");
            this.Events.Register<AppliedForce>("force:applied");
            this.Events.Register<Category>("collision-categories:changed");
            this.Events.Register<Category>("collides-with:changed");
            this.Events.Register<Category>("ignore-ccd-with:changed");
            this.Events.Register<Boolean>("reserved:changed");
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Build a new body for the entity.
            this.CreateBody();
        }

        public override void Dispose()
        {
            base.Dispose();

            this.DestroyBody();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Build the farseer body to be used as the entities main body.
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        protected abstract Body BuildBody(World world);

        /// <summary>
        /// Create and save the entities main body
        /// </summary>
        /// <param name="world"></param>
        private void CreateBody()
        {
            this.body = this.BuildBody(_world);
            this.Events.TryInvoke<Body>(this, "body:created", this.body);
        }

        /// <summary>
        /// Destroy the entity's body.
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        private void DestroyBody()
        {
            // Ensure that all fixtures are destroyed
            this.DestroyAllFixtures();

            this.body.Dispose();
            this.Events.TryInvoke<Body>(this, "body:destroyed", this.body);
        }
        #endregion

        #region Farseer Methods
        /// <summary>
        /// Create a new fixture within the farseer entity
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public virtual Fixture CreateFixture(Shape shape, Object userData = null, Action<Fixture> setup = null)
        {
            // Create the new fixture...
            var fixture = this.body.CreateFixture(shape, userData);
            setup?.Invoke(fixture);

            // Invoke the created event...
            this.Events.TryInvoke<Fixture>(this, "fixture:created", fixture);

            return fixture;
        }

        /// <summary>
        /// Destroy a specified fixture.
        /// </summary>
        /// <param name="fixture"></param>
        public virtual void DestroyFixture(Fixture fixture)
        {
            fixture.Body.DestroyFixture(fixture);

            // Invoke the destroyed event...
            this.Events.TryInvoke<Fixture>(this, "fixture:destroyed", fixture);
        }

        /// <summary>
        /// Destroy all fixtures contained within the current body.
        /// </summary>
        protected virtual void DestroyAllFixtures()
        {
            // Remove all pre existing fixtures...
            if(this.body.FixtureList != null)
                while (this.body.FixtureList.Any())
                    this.DestroyFixture(this.body.FixtureList.First());
        }

        /// <summary>
        /// Update the bodies position via SetTransform and invoke
        /// the position:updated event
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void SetPosition(Vector2 position, Single rotation, Boolean ignoreContacts = false)
        {
            this.SetPosition(ref position, rotation, ignoreContacts);
        }
        /// <summary>
        /// Update the bodies position via SetTransform and invoke
        /// the position:updated event
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void SetPosition(ref Vector2 position, Single rotation, Boolean ignoreContacts = false)
        {
            if(ignoreContacts)
                this.body.SetTransformIgnoreContacts(ref position, rotation);
            else
                this.body.SetTransform(ref position, rotation);

            this.Events.TryInvoke<Body>(this, "position:changed", this.body);
        }

        /// <summary>
        /// Update the bodies velocity and invoke
        /// the velocity:updated event
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void SetVelocity(Vector2 linear, Single angular)
        {
            this.body.LinearVelocity = linear;
            this.body.AngularVelocity = angular;

            this.Events.TryInvoke<Body>(this, "velocity:changed", this.body);
        }

        /// <summary>
        /// Apply an impulse at a point. This immediately modifies the velocity.
        /// This wakes up the body.
        /// </summary>
        /// <param name="impulse">The world impulse vector, usually in N-seconds or kg-m/s.</param>
        public void ApplyLinearImpulse(Vector2 impulse)
        {
            this.body.ApplyLinearImpulse(impulse);

            this.Events.TryInvoke<Vector2>(this, "linear-impulse:applied", impulse);
        }

        /// <summary>
        /// Apply an angular impulse.
        /// </summary>
        /// <param name="impulse">The angular impulse in units of kg*m*m/s.</param>
        public void ApplyAngularImpulse(Single impulse)
        {
            this.body.ApplyAngularImpulse(impulse);

            this.Events.TryInvoke<Single>(this, "angular-impulse:applied", impulse);
        }

        /// <summary>
        /// Apply a force at a world point. If the force is not
        /// applied at the center of mass, it will generate a torque and
        /// affect the angular velocity. This wakes up the body.
        /// </summary>
        /// <param name="force">The world force vector, usually in Newtons (N).</param>
        /// <param name="point">The world position of the point of application.</param>
        public void ApplyForce(Vector2 force, Vector2 point)
        {
            this.body.ApplyForce(force, point);

            var appliedForce = _forcePool.Pull(t => new AppliedForce());
            appliedForce.Force = force;
            appliedForce.Point = point;
            this.Events.TryInvoke<AppliedForce>(this, "force:applied", appliedForce);
            _forcePool.Put(appliedForce);
        }

        /// <summary>
        /// Update the bodies collision category 
        /// </summary>
        /// <param name="category"></param>
        public void SetCollidesWith(Category category)
        {
            this.body.CollidesWith = category;
            this.CollidesWith = category;

            this.Events.TryInvoke<Category>(this, "collides-with:changed", this.CollidesWith);
        }

        public void SetCollisionCategories(Category category)
        {
            this.body.CollisionCategories = category;
            this.CollisionCategories = category;

            this.Events.TryInvoke<Category>(this, "collision-categories:changed", this.CollisionCategories);
        }

        public void SetIgnoresCCDWith(Category category)
        {
            this.body.IgnoreCCDWith = category;
            this.IgnoreCCDWith = category;

            this.Events.TryInvoke<Category>(this, "ignore-ccd-with:changed", this.IgnoreCCDWith);
        }

        public void SetBodyEnabled(Boolean value)
        {
            if(value != this.body.Enabled)
            {
                this.body.Enabled = value;

                this.Events.TryInvoke<Boolean>(this, "body-enabled:changed", this.BodyEnabled);
            }
        }
        #endregion

        #region Network Methods
        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            this.body.ReadPosition(im);
            this.body.ReadVelocity(im);
        }

        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            this.body.WritePosition(om);
            this.body.WriteVelocity(om);
        }

        protected override void ReadPostInitialize(NetIncomingMessage im)
        {
            base.ReadPostInitialize(im);

            this.body.ReadPosition(im);
            this.body.ReadVelocity(im);
        }

        protected override void WritePostInitialize(NetOutgoingMessage om)
        {
            base.WritePostInitialize(om);

            this.body.WritePosition(om);
            this.body.WriteVelocity(om);
        }
        #endregion
    }
}
