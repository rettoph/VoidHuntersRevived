using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Lists;
using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Extensions.Microsoft.Xna;
using Guppy.Extensions.DependencyInjection;
using Guppy.IO.Services;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Entities
{
    /// <summary>
    /// Simple entity used to interact with the farseer world.
    /// </summary>
    public class Sensor : Entity
    {
        #region Private Fields
        private MouseService _mouse;
        private WorldEntity _world;
        private FarseerCamera2D _camera;
        private Body _body;
        private HashSet<BodyEntity> _contacts;
        private GameScene _scene;
        #endregion

        #region Public Attributes
        public IEnumerable<BodyEntity> Contacts => _contacts;
        public Vector2 Position => _body.WorldCenter;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _mouse);
            provider.Service(out _camera);
            provider.Service(out _scene);

            _contacts = new HashSet<BodyEntity>();

            _scene.IfOrOnWorld(this.ConfigureFarseer);

            this.LayerGroup = 10;
        }

        protected override void Release()
        {
            base.Release();

            _body?.Dispose();

            this.OnUpdate -= this.UpdateBody;
        }
        #endregion

        #region Frame Methods
        private void UpdateBody(GameTime gameTime)
        {
            var position = _camera.Unproject(_mouse.Position.ToVector3());
            _body.SetTransform(position.ToVector2(), 0);
        }
        #endregion

        #region Event Handlers
        private void ConfigureFarseer(WorldEntity world)
        {
            _world = world;

            _body = BodyFactory.CreateCircle(_world.Slave, 3f, 0f);
            _body.IsSensor = true;
            _body.SleepingAllowed = false;
            _body.BodyType = BodyType.Dynamic;

            _body.OnCollision += this.HandleSensorCollision;
            _body.OnSeparation += this.HandleSensorSeparation;

            this.OnUpdate += this.UpdateBody;
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
