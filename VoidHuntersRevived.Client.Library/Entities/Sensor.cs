using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.UI.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Entities
{
    /// <summary>
    /// The sensor represents a circle area of selectable
    /// objects to the current user player
    /// </summary>
    public class Sensor : Entity
    {
        #region Private Fields
        private ClientWorldScene _scene;
        private Pointer _pointer;
        private World _world;
        private Body _body;
        private HashSet<FarseerEntity> _contacts;
        private FarseerCamera2D _camera;
        #endregion

        #region Public Attributes
        /// <summary>
        /// Public list of all ShipPart contacts tracked by the sensor.
        /// </summary>
        public IEnumerable<FarseerEntity> Contacts { get { return _contacts; } }
        public Vector2 WorldCenter { get { return _body.WorldCenter; } }
        #endregion

        #region Constructor
        public Sensor(ContentManager content, ClientWorldScene scene, FarseerCamera2D camera, Pointer pointer, World world)
        {
            _scene = scene;
            _pointer = pointer;
            _world = world;
            _camera = camera;
            _contacts = new HashSet<FarseerEntity>();
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();
            // Create a new body for the sensor
            _body = BodyFactory.CreateCircle(_world, 3f, 0f);
            _body.IsSensor = true;
            _body.SleepingAllowed = false;
            _body.BodyType = BodyType.Dynamic;
            // _body.CollisionCategories = Categories.BorderCollisionCategory;
            // _body.CollidesWith = Categories.BorderCollidesWith;

            _body.OnCollision += this.HandleSensorCollision;
            _body.OnSeparation += this.HandleSensorSeparation;
        }

        public override void Dispose()
        {
            base.Dispose();

            _body.Dispose();
            _contacts.Clear();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var position = _camera.Unproject(new Vector3(_pointer.Position, 0));
            _body.SetTransform(new Vector2(position.X, position.Y), 0);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        #endregion

        #region Event Handlers
        private bool HandleSensorCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureA.UserData is FarseerEntity)
                _contacts.Add(fixtureA.UserData as FarseerEntity);
            if (fixtureB.UserData is FarseerEntity)
                _contacts.Add(fixtureB.UserData as FarseerEntity);

            return true;
        }

        private void HandleSensorSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            if (fixtureA.UserData is FarseerEntity)
                _contacts.Remove(fixtureA.UserData as FarseerEntity);
            if (fixtureB.UserData is FarseerEntity)
                _contacts.Remove(fixtureB.UserData as FarseerEntity);
        }
        #endregion
    }
}
