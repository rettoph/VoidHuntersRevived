using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Collections;
using Guppy.DependencyInjection;
using Guppy.UI.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Extensions.Microsoft.Xna;

namespace VoidHuntersRevived.Client.Library.Entities
{
    /// <summary>
    /// Simple entity used to interact with the farseer world.
    /// </summary>
    public class Sensor : Entity
    {
        #region Private Fields
        private EntityCollection _entities;
        private Cursor _cursor;
        private WorldEntity _world;
        private FarseerCamera2D _camera;
        private Body _body;
        private HashSet<BodyEntity> _contacts;
        #endregion

        #region Public Attributes
        public IEnumerable<BodyEntity> Contacts => _contacts;
        public Vector2 Position => _body.WorldCenter;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _entities);
            provider.Service(out _cursor);
            provider.Service(out _camera);

            _contacts = new HashSet<BodyEntity>();

            _entities.OnAdded += this.HandleEntityAdded;
        }

        protected override void Dispose()
        {
            base.Dispose();

            _body?.Dispose();

            this.OnUpdate -= this.UpdateBody;
            _entities.OnAdded -= this.HandleEntityAdded;
        }
        #endregion

        #region Frame Methods
        private void UpdateBody(GameTime gameTime)
        {
            var position = _camera.Unproject(_cursor.Position.ToVector3());
            _body.SetTransform(position.ToVector2(), 0);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Wait for the world to  be created before
        /// finalizing initialization.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleEntityAdded(IEnumerable<Entity> sender, Entity arg)
        {
            if(arg is WorldEntity)
            {
                _world = arg as WorldEntity;
                _entities.OnAdded -= this.HandleEntityAdded;

                _body = BodyFactory.CreateCircle(_world.Slave, 3f, 0f);
                _body.IsSensor = true;
                _body.SleepingAllowed = false;
                _body.BodyType = BodyType.Dynamic;

                _body.OnCollision += this.HandleSensorCollision;
                _body.OnSeparation += this.HandleSensorSeparation;

                this.OnUpdate += this.UpdateBody;
            }
        }

        private bool HandleSensorCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureA.UserData is BodyEntity)
                _contacts.Add(fixtureA.UserData as BodyEntity);
            if (fixtureB.UserData is BodyEntity)
                _contacts.Add(fixtureB.UserData as BodyEntity);

            return true;
        }

        private void HandleSensorSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            if (fixtureA.UserData is BodyEntity)
                _contacts.Remove(fixtureA.UserData as BodyEntity);
            if (fixtureB.UserData is BodyEntity)
                _contacts.Remove(fixtureB.UserData as BodyEntity);
        }
        #endregion
    }
}
