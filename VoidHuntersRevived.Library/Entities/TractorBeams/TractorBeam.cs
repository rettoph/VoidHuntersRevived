using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;

namespace VoidHuntersRevived.Library.Entities.TractorBeams
{
    /// <summary>
    /// A tractor beam is a simple item that acts as a players
    /// mouse, allowing them to pick up objects
    /// </summary>
    public class TractorBeam : FarseerEntity
    {
        public TractorBeam(EntityInfo info, IGame game) : base(info, game)
        {
        }

        protected override void HandleAddedToScene(object sender, ISceneObject e)
        {
            base.HandleAddedToScene(sender, e);

            this.Body.CreateFixture(new CircleShape(2, 0f));
            this.Body.CollisionCategories = Category.None;
            this.Body.BodyType = BodyType.Dynamic;
        }

        private bool HandleCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            return Mouse.GetState().LeftButton == ButtonState.Pressed;
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }
    }
}
