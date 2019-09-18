using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using GalacticFighters.Client.Library.Scenes;
using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Utilities;
using Guppy;
using Guppy.UI.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Entities
{
    /// <summary>
    /// The sensor represents a circle area of selectable
    /// objects to the current user player
    /// </summary>
    public class Sensor : Entity
    {
        #region Private Fields
        private ClientGalacticFightersWorldScene _scene;
        private Pointer _pointer;
        private World _world;
        private Body _body;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private HashSet<ShipPart> _contacts;
        #endregion

        #region Public Attributes
        /// <summary>
        /// Public list of all ShipPart contacts tracked by the sensor.
        /// </summary>
        public IEnumerable<ShipPart> Contacts { get { return _contacts; } }
        public Vector2 WorldCenter { get { return _body.WorldCenter; } }
        #endregion

        #region Constructor
        public Sensor(SpriteBatch spriteBatch, ContentManager content, ClientGalacticFightersWorldScene scene, Pointer pointer, World world)
        {
            _spriteBatch = spriteBatch;
            _font = content.Load<SpriteFont>("font");
            _scene = scene;
            _pointer = pointer;
            _world = world;
            _contacts = new HashSet<ShipPart>();
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
            _body.CollisionCategories = CollisionCategories.BorderCollisionCategory;
            _body.CollidesWith = CollisionCategories.BorderCollidesWith;

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

            var position = _scene.Camera.Unproject(new Vector3(_pointer.Position, 0));
            _body.SetTransform(new Vector2(position.X, position.Y), 0);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.DrawString(_font, $"Sensor Contacts: {_contacts.Count}", new Vector2(15, 95), Color.White);
        }
        #endregion

        #region Event Handlers
        private bool HandleSensorCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureA.UserData is ShipPart)
                _contacts.Add(fixtureA.UserData as ShipPart);
            if (fixtureB.UserData is ShipPart)
                _contacts.Add(fixtureB.UserData as ShipPart);

            return true;
        }

        private void HandleSensorSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            if (fixtureA.UserData is ShipPart)
                _contacts.Remove(fixtureA.UserData as ShipPart);
            if (fixtureB.UserData is ShipPart)
                _contacts.Remove(fixtureB.UserData as ShipPart);
        }
        #endregion
    }
}
