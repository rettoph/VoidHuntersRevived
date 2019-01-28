using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Interfaces;

namespace VoidHuntersRevived.Client.Entities
{
    public class Cursor : FarseerEntity
    {
        private ClientMainScene _scene;
        private MouseState _mouse;

        private List<IFarseerEntity> _contacts;

        public IFarseerEntity Over;

        public Cursor(EntityInfo info, IGame game) : base(info, game)
        {
            this.Enabled = true;
            this.Visible = false;

            _contacts = new List<IFarseerEntity>();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _mouse = Mouse.GetState();

            this.Body.Position = Vector2.Transform(
                ConvertUnits.ToSimUnits(_mouse.Position.ToVector2()),
                _scene.Camera.InverseViewMatrix);

            this.Over = _contacts
                .OrderBy(e => Vector2.Distance(e.Body.Position, this.Body.Position))
                .FirstOrDefault();
        }

        protected override void Initialize()
        {
            base.Initialize();

            _scene = this.Scene as ClientMainScene;

            this.Body.CreateFixture(
                new CircleShape(2, 0f));

            this.Body.SleepingAllowed = false;
            this.Body.BodyType = BodyType.Dynamic;
            this.Body.IsSensor = true;

            this.World.ContactManager.BeginContact += this.HandleBeginContact;
            this.World.ContactManager.EndContact += this.HandleEndContact;
        }

        private void HandleEndContact(Contact contact)
        {
            if (contact.FixtureB.Body == this.Body)
                if (contact.FixtureA.Body.UserData is IFarseerEntity)
                    _contacts.Remove(contact.FixtureA.Body.UserData as IFarseerEntity);
        }

        private bool HandleBeginContact(Contact contact)
        {
            if(contact.FixtureB.Body == this.Body)
                if (contact.FixtureA.Body.UserData is IFarseerEntity)
                {
                    var entity = contact.FixtureA.Body.UserData as IFarseerEntity;

                    if (!_contacts.Contains(entity))
                        _contacts.Add(entity);
                }
                        

            return true;
        }
    }
}
