﻿using FarseerPhysics.Collision.Shapes;
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
        private Category _collidesWith;
        private Category _collisionCategories;

        private Dictionary<Shape, Fixture> _shapeFixtureTable;
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
        }
        public Category CollidesWith
        {
            get { return _collidesWith; }
            set
            {
                _collidesWith = value;
                _body.CollidesWith = _collidesWith;
                this.OnCollidesWithChanged?.Invoke(this, _collidesWith);
            }
        }
        public Category CollisionCategories
        {
            get { return _collisionCategories; }
            set
            {
                _collisionCategories = value;
                _body.CollisionCategories = _collisionCategories;
                this.OnCollisionCategoriesChanged?.Invoke(this, _collisionCategories);
            }
        }
        #endregion

        #region Events
        public event EventHandler<Category> OnCollidesWithChanged;
        public event EventHandler<Category> OnCollisionCategoriesChanged;
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

            _collidesWith = Category.All;
            _collisionCategories = Category.Cat1;
            _shapeFixtureTable = new Dictionary<Shape, Fixture>();
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

        public Shape CreateFixture(Shape shape)
        {
            var fixture = _body.CreateFixture(shape, this);
            _shapeFixtureTable.Add(shape, fixture);

            // Update the fixture collision categories
            fixture.CollidesWith = this.CollidesWith;
            fixture.CollisionCategories = this.CollisionCategories;

            this.OnFixtureCreated?.Invoke(this, shape);

            return shape;
        }

        public void DestroyFixture(Shape shape)
        {
            if (!_shapeFixtureTable.ContainsKey(shape))
                throw new Exception("Unable to destroy fixture, shape unknown.");

            _body.DestroyFixture(_shapeFixtureTable[shape]);
            _shapeFixtureTable.Remove(shape);

            this.OnFixtureDestroyed?.Invoke(this, shape);
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

        public override void Dispose()
        {
            base.Dispose();

            _body.Dispose();
        }
    }
}
