using Guppy.DependencyInjection;
using Guppy.DependencyInjection.ServiceConfigurations;
using Guppy.Extensions.System;
using Guppy.Lists;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Extensions.Aether;

namespace VoidHuntersRevived.Library.Entities.Aether
{
    public class AetherBody : BaseAetherWrapper<Body>
    {
        #region Private Fields
        private FactoryServiceList<AetherFixture> _fixtures;
        #endregion

        #region Public Properties
        /// <summary>
        /// The owning <see cref="AetherWorld"/> instance. This will autmatically be defined
        /// on creation.
        /// </summary>
        public AetherWorld World { get; internal set; }

        /// <summary>
        /// Set the user data. Use this to store your application specific data.
        /// </summary>
        public Object Tag
        {
            set => this.Do(body => body.Tag = value);
        }

        /// <summary>
        /// Get the local position of the center of mass.
        /// Warning: This property is readonly during callbacks.
        /// </summary>
        /// <value>The local position.</value>
        /// <exception cref="System.InvalidOperationException">Thrown when the world is Locked/Stepping.</exception>
        public Vector2 LocalCenter
        {
            get => this.LocalInstance.LocalCenter;
            set => this.LocalInstance.LocalCenter = value;
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _fixtures);
        }

        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            if (this.World == default)
                throw new Exception("Improperly constructed AetherBody detected. Please ensure you call AetherWorld.CreateBody.");
        }

        protected override void PostRelease()
        {
            _fixtures.TryRelease();
            _fixtures = default;

            this.Do((auth, body) => this.World.BodyFactories[auth].TryReturnToPool(body));

            base.PostRelease();
        }
        #endregion

        #region Helper Methods
        protected override Body BuildInstance(GuppyServiceProvider provider, NetworkAuthorization authorization)
            => this.World.BodyFactories[authorization].Create();
        #endregion

        #region CreateFixture Methods
        /// <summary>
        /// Create a new AetherBody instance linked to this world.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public AetherFixture CreateFixture(Shape shape, Object tag)
        {
            return _fixtures.Create((fixture, provider, _) =>
            {
                fixture.Body = this;
                fixture.Shape = shape;
                fixture.BuildAetherInstances(provider);

                fixture.Tag = tag;
            });
        }

        /// <summary>
        /// Create a new AetherBody instance linked to this world.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public AetherFixture CreateFixture(Shape shape, Action<AetherFixture, GuppyServiceProvider, IServiceConfiguration> setup)
        {
            return _fixtures.Create((fixture, provider, configuration) =>
            {
                fixture.Body = this;
                fixture.Shape = shape;
                fixture.BuildAetherInstances(provider);

                setup(fixture, provider, configuration);
            });
        }
        #endregion

        #region Body Methods
        /// <summary>
        /// Apply an impulse at a point. This immediately modifies the velocity.
        /// This wakes up the body.
        /// </summary>
        /// <param name="impulse">The world impulse vector, usually in N-seconds or kg-m/s.</param>
        public void ApplyLinearImpulse(Vector2 impulse)
            => this.DoPlaftorm(b => b.ApplyLinearImpulse(impulse));

        public void SetTransformIgnoreContacts(Vector2 position, Single angle)
            => this.DoPlaftorm(b => b.SetTransformIgnoreContacts(ref position, angle));
        #endregion
    }
}
