﻿using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Implementations;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    public class ClientFarseerEntityDriver : Driver
    {
        private Dictionary<Shape, Fixture> _serverShapeFixtureTable;
        private Body _serverBody;
        private VoidHuntersClientWorldScene _scene;
        private FarseerEntity _entity;
        private Single _lerpStrength;

        #region Constructors
        public ClientFarseerEntityDriver(VoidHuntersClientWorldScene scene, FarseerEntity entity, IServiceProvider provider, ILogger logger) : base(entity, provider, logger)
        {
            _scene = scene;
            _entity = entity;
        }
        #endregion

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            _serverShapeFixtureTable = new Dictionary<Shape, Fixture>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Bind action handlers
            _entity.ActionHandlers.Add("update:position", this.HandleUpdatePositionAction);

            // Bind event handlers
            _entity.OnCollidesWithChanged += this.HandleCollidesWithChanged;
            _entity.OnCollisionCategoriesChanged += this.CollidesCollisionCategoriesChanged;
            _entity.OnFixtureCreated += this.HandleFixtureCreated;
            _entity.OnFixtureDestroyed += this.HandleFixtureDestroyed;
            _entity.OnLinearImpulseApplied += this.HandleLinearImpulseApplied;
            _entity.OnAngularImpulseApplied += this.HandleAngularImpulseApplied;
            _entity.OnRead += this.HandleRead;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _lerpStrength = 0.05f;

            // Create a new body within the server world to represent the server render of the current entity
            _serverBody = _entity.CreateBody(_scene.ServerWorld, _entity.Position, _entity.Rotation);
        }
        #endregion

        #region Frame Methods
        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if(_serverBody.Awake)
            {
                _entity.Position = Vector2.Lerp(_entity.Position, _serverBody.Position, _lerpStrength);
                _entity.Rotation = MathHelper.Lerp(_entity.Rotation, _serverBody.Rotation, _lerpStrength);
                _entity.LinearVelocity = Vector2.Lerp(_entity.LinearVelocity, _serverBody.LinearVelocity, _lerpStrength);
                _entity.AngularVelocity = MathHelper.Lerp(_entity.AngularVelocity, _serverBody.AngularVelocity, _lerpStrength);
            }
        }
        #endregion

        #region Action Handlers
        private void HandleUpdatePositionAction(NetIncomingMessage obj)
        {
            _serverBody.Position = obj.ReadVector2();
            _serverBody.Rotation = obj.ReadSingle();
            _serverBody.LinearVelocity = obj.ReadVector2();
            _serverBody.AngularVelocity = obj.ReadSingle();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the client render recieves a fixture,
        /// duplocate it on the server render too
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shape"></param>
        private void HandleFixtureCreated(object sender, Shape shape)
        {
            var fixture = _serverBody.CreateFixture(shape);
            fixture.CollidesWith = _entity.CollidesWith;
            fixture.CollisionCategories = _entity.CollisionCategories;

            _serverShapeFixtureTable.Add(shape, fixture);
        }

        /// <summary>
        /// When the client render destroyed a fixture,
        /// duplicate it on the server render too
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shape"></param>
        private void HandleFixtureDestroyed(object sender, Shape shape)
        {
            _serverBody.DestroyFixture(_serverShapeFixtureTable[shape]);
            _serverShapeFixtureTable.Remove(shape);
        }

        private void HandleAngularImpulseApplied(object sender, float impulse)
        {
            _serverBody.ApplyAngularImpulse(impulse);
        }

        private void HandleLinearImpulseApplied(object sender, Vector2 impulse)
        {
            _serverBody.ApplyLinearImpulse(impulse);
        }

        private void CollidesCollisionCategoriesChanged(object sender, Category category)
        {
            _serverBody.CollisionCategories = category;
        }

        private void HandleCollidesWithChanged(object sender, Category category)
        {
            _serverBody.CollidesWith = category;
        }

        private void HandleRead(object sender, NetworkEntity e)
        {
            _serverBody.Position = _entity.Position;
            _serverBody.Rotation = _entity.Rotation;
            _serverBody.LinearVelocity = _entity.LinearVelocity;
            _serverBody.AngularVelocity = _entity.AngularVelocity;
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            _serverBody.Dispose();

            _entity.OnCollidesWithChanged -= this.HandleCollidesWithChanged;
            _entity.OnCollisionCategoriesChanged -= this.CollidesCollisionCategoriesChanged;
            _entity.OnFixtureCreated -= this.HandleFixtureCreated;
            _entity.OnFixtureDestroyed -= this.HandleFixtureDestroyed;
            _entity.OnLinearImpulseApplied -= this.HandleLinearImpulseApplied;
            _entity.OnAngularImpulseApplied -= this.HandleAngularImpulseApplied;
            _entity.OnRead -= this.HandleRead;
        }
    }
}
