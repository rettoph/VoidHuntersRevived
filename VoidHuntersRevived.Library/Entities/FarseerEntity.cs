using Guppy;
using Guppy.Collections;
using Guppy.Configurations;
using Guppy.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Drivers;
using Microsoft.Extensions.DependencyInjection;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Farseer entities are simple entity implementations
    /// that contain farseer bodys and contain custom drivers
    /// for client to server communications
    /// </summary>
    public abstract class FarseerEntity : NetworkEntity
    {
        private IServiceProvider _provider;
        private FarseerEntityDriver _driver;

        public Body Body { get; private set; }

        #region Constructor Methods
        public FarseerEntity(EntityConfiguration configuration, Scene scene, ILogger logger, IServiceProvider provider) : base(configuration, scene, logger)
        {
            _provider = provider;
        }
        public FarseerEntity(Guid id, EntityConfiguration configuration, Scene scene, ILogger logger, IServiceProvider provider) : base(id, configuration, scene, logger)
        {
            _provider = provider;
        }
        #endregion

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            this.Body = BodyFactory.CreateBody(
                world: _provider.GetService<World>(),
                userData: this,
                bodyType: BodyType.Dynamic);
            this.Body.AngularDamping = 1f;
            this.Body.LinearDamping = 1f;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Build the entity's driver...
            _driver = _provider.GetService<EntityCollection>().Create<FarseerEntityDriver>("driver:farseer-entity", this);
        }
        #endregion

        #region Frame Methods
        public override void Draw(GameTime gameTime)
        {
            _driver.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            _driver.Update(gameTime);
        }
        #endregion

        #region Farseer Methods
        public virtual void ApplyLinearImpulse(Vector2 impulse)
        {
            _driver.ApplyLinearImpulse(impulse);
        }

        public virtual void ApplyAngularImpulse(Single impulse)
        {
            _driver.ApplyAngularImpulse(impulse);
        }

        public virtual void CreateFixture(Shape shape)
        {
            // Alert the farseer entity driver to create a fixture
            _driver.CreateFixture(shape);
        }
        #endregion
    }
}
