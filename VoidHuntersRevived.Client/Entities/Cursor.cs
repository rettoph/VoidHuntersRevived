using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using System.Linq;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics.Contacts;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Interfaces;

namespace VoidHuntersRevived.Client.Entities
{
    /// <summary>
    /// The client side cursor is an entity that contains a single
    /// farseer body (circle) to act as a sensor. The cursor will
    /// allow a single object to be hovered at any time, and the
    /// hovered object can be selected via the GetTarget() method
    /// within the cursor.
    /// 
    /// Other entities, such as the local UserPlayer tractor beam may
    /// be controlled via the cursor as well
    /// </summary>
    public class Cursor : Entity
    {
        #region Private Fields
        private Body _body;
        private Camera _camera;

        private World _world;

        private List<IFarseerEntity> _contactList;

        private IFarseerEntity _target;
        #endregion

        

        public Cursor(EntityInfo info, IGame game) : base(info, game)
        {
            _contactList = new List<IFarseerEntity>();
        }

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            var scene = this.Scene as ClientMainGameScene;

            // Save the scenes camera
            _camera = scene.Camera;
            // Save the scenes world
            _world = scene.World;

            // Create a new body
            _body = BodyFactory.CreateCircle(
                _world,
                1f,
                0f,
                Vector2.Zero,
                BodyType.Dynamic);

            // Mark the cursor as a sensor
            _body.IsSensor = true;
            _body.SleepingAllowed = false;

            // Ensure the cursor is enabled
            this.SetEnabled(true);

            // Add event listeners
            _body.OnCollision += this.HandleCollision;
            _body.OnSeparation += this.HandleSeperation;
        }
        #endregion

        public IFarseerEntity GetTarget()
        {
            return _target;
        }

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the Cursor's position
            _body.Position = Vector2.Transform(ConvertUnits.ToSimUnits(Mouse.GetState().Position.ToVector2()), _camera.InverseViewMatrix);

            // Select the current Cursor's target
            _target = _contactList.OrderBy(fe => Vector2.Distance(_body.Position, fe.Body.Position)).FirstOrDefault();
        }
        #endregion

        #region Event Handlers
        private bool HandleCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if(fixtureB.UserData is IFarseerEntity)
                _contactList.Add(fixtureB.UserData as IFarseerEntity);

            return true;
        }

        private void HandleSeperation(Fixture fixtureA, Fixture fixtureB)
        {
            if (fixtureB.UserData is IFarseerEntity)
                _contactList.Remove(fixtureB.UserData as IFarseerEntity);
        }
        #endregion
    }
}
