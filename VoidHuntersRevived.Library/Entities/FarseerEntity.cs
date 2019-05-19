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

        public FarseerEntity(EntityConfiguration configuration, Scene scene, ILogger logger, IServiceProvider provider) : base(configuration, scene, logger)
        {
            _provider = provider;
        }
        public FarseerEntity(Guid id, EntityConfiguration configuration, Scene scene, ILogger logger, IServiceProvider provider) : base(id, configuration, scene, logger)
        {
            _provider = provider;
        }

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

        public override void Draw(GameTime gameTime)
        {
            _driver.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            _driver.Update(gameTime);
        }

    }
}
