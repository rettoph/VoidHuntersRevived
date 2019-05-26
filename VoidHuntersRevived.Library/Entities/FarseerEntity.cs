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
        private Dictionary<Shape, Fixture> _shapeFixtureTable;
        public Body Body { get; private set; }

        #region Events
        public event EventHandler<Shape> OnFixtureCreated;
        public event EventHandler<Shape> OnFixtureDestroyed;
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

            _shapeFixtureTable = new Dictionary<Shape, Fixture>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.Body = this.CreateBody((this.scene as VoidHuntersWorldScene).World);
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

        public PolygonShape CreateFixture(PolygonShape shape)
        {
            var fixture = this.Body.CreateFixture(shape, this);
            _shapeFixtureTable.Add(shape, fixture);

            this.OnFixtureCreated?.Invoke(this, shape);

            return shape;
        }

        public void DestroyFixture(Shape shape)
        {
            if (!_shapeFixtureTable.ContainsKey(shape))
                throw new Exception("Unable to destroy fixture, shape unknown.");

            this.Body.DestroyFixture(_shapeFixtureTable[shape]);
            _shapeFixtureTable.Remove(shape);

            this.OnFixtureDestroyed?.Invoke(this, shape);
        }

        public void ApplyLinearImpulse(Vector2 impulse)
        {
            this.Body.ApplyLinearImpulse(impulse);

            this.OnLinearImpulseApplied?.Invoke(this, impulse);
        }

        public void ApplyAngularImpulse(Single impulse)
        {
            this.Body.ApplyAngularImpulse(impulse);

            this.OnAngularImpulseApplied?.Invoke(this, impulse);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            this.Body.Dispose();
        }
    }
}
