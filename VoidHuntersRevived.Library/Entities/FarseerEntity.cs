using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GalacticFighters.Library.Extensions.Farseer;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticFighters.Library.Entities
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

        /// <summary>
        /// The raw body managed by this FarseerEntity. It is not recommened that
        /// you interact with the body derectly, but use the interface available to
        /// you within the FarseerEntity.
        /// </summary>
        private Body _body;
        #endregion

        #region Public Attributes
        public Boolean Awake { get { return _body.Awake; } }

        /// <summary>
        /// Get the world body origin position.
        /// </summary>
        public Vector2 Position { get { return _body.Position; } }

        /// <summary>
        /// Get the angle in radians.
        /// </summary>
        public Single Rotation { get { return _body.Rotation; } }

        /// <summary>
        /// Get the linear velocity of the center of mass.
        /// </summary>
        public Vector2 LinearVelocity { get { return _body.LinearVelocity; } }

        /// <summary>
        /// Gets the angular velocity. Radians/second.
        /// </summary>
        public Single AngularVelocity { get { return _body.AngularVelocity; } }

        /// <summary>
        /// Get the world position of the center of mass.
        /// </summary>
        /// <value>The world position.</value>
        public Vector2 WorldCenter { get { return _body.WorldCenter; } }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _world = provider.GetRequiredService<World>();

            // Initialize basic events
            this.Events.Register<Body>("body:created");
            this.Events.Register<Body>("body:destroyed");
            this.Events.Register<Fixture>("fixture:created");
            this.Events.Register<Fixture>("fixture:destroyed");

            this.Events.Register<Body>("position:changed");
            this.Events.Register<Body>("velocity:changed");
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

            // if (!this.Awake) // By default, sleeping entities are disabled
            //     this.SetEnabled(false);
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
            _body = this.BuildBody(_world);
            this.Events.TryInvoke<Body>(this, "body:created", _body);
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

            _body.Dispose();
            this.Events.TryInvoke<Body>(this, "body:destroyed", _body);
        }
        #endregion

        #region Farseer Methods
        /// <summary>
        /// Create a new fixture within the farseer entity
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public virtual Fixture CreateFixture(Shape shape, Object userData = null)
        {
            // Create the new fixture...
            var fixture = _body.CreateFixture(shape, userData);

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
            while (_body.FixtureList.Any())
                this.DestroyFixture(_body.FixtureList.First());
        }

        /// <summary>
        /// Update the bodies position via SetTransform and invoke
        /// the position:updated event
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void SetPosition(Vector2 position, Single rotation)
        {
            this.SetPosition(ref position, rotation);
        }
        /// <summary>
        /// Update the bodies position via SetTransform and invoke
        /// the position:updated event
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void SetPosition(ref Vector2 position, Single rotation)
        {
            _body.SetTransform(ref position, rotation);

            this.Events.TryInvoke<Body>(this, "position:changed", _body);
        }

        /// <summary>
        /// Update the bodies velocity and invoke
        /// the velocity:updated event
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void SetVelocity(Vector2 linear, Single angular)
        {
            _body.LinearVelocity = linear;
            _body.AngularVelocity = angular;

            this.Events.TryInvoke<Body>(this, "velocity:changed", _body);
        }
        #endregion

        #region Network Methods
        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            _body.ReadPosition(im);
            _body.ReadVelocity(im);
        }

        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            _body.WritePosition(om);
            _body.WriteVelocity(om);
        }
        #endregion
    }
}
