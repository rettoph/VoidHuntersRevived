using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Configurations;
using Guppy.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Entity representing an object within the farseer world.
    /// </summary>
    public class FarseerEntity : NetworkEntity
    {
        public Body Body { get; private set; }

        #region Events
        public event EventHandler<Fixture> OnFixtureCreated;
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
        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.Body = this.CreateBody((this.scene as VoidHuntersWorldScene).World);

            this.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(1, 1), 1f));
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
            float rotation = 0, 
            BodyType bodyType = BodyType.Static)
        {
            var body = BodyFactory.CreateBody(
                world,
                position,
                rotation,
                bodyType,
                this);

            body.LinearDamping = 1f;
            body.AngularDamping = 1f;

            return body;
        }

        public Fixture CreateFixture(Shape shape)
        {
            var fixture = this.Body.CreateFixture(shape, this);

            this.OnFixtureCreated?.Invoke(this, fixture);

            return fixture;
        }
        #endregion
    }
}
