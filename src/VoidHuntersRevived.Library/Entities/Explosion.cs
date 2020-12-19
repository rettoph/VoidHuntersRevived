using FarseerPhysics.Collision.Shapes;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Utilities.Farseer;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Guppy.Extensions.Utilities;
using FarseerPhysics.Common;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// A simple class that will manage an
    /// expanding circle and apply forces to 
    /// colliding <see cref="Chain"/> instances.
    /// </summary>
    public class Explosion : BodyEntity
    {
        #region Private Fields
        private FixtureContainer _fixtures;
        private List<Contact> _contacts;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _primitiveBatch);
            _contacts = new List<Contact>();
            _fixtures = this.BuildFixture(new CircleShape(1f, 0f));

            this.Do(b =>
            {
                b.IsSensor = true;
                b.SleepingAllowed = false;
                b.BodyType = BodyType.Dynamic;
                b.CollisionCategories = Categories.BorderCollisionCategories;
                b.CollidesWith = Categories.BorderCollidesWith;
                b.IgnoreCCDWith = Categories.BorderIgnoreCCDWith;

                b.OnCollision += this.HandleSensorCollision;
            });
        }

        protected override void Release()
        {
            base.Release();

            this.Do(b =>
            {
                b.OnCollision -= this.HandleSensorCollision;
            });
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _fixtures.List.ForEach(f =>
            {
                if(f.Shape is CircleShape c)
                {
                    c.Radius += (Single)gameTime.ElapsedGameTime.TotalSeconds / 5;
                }
            });
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _contacts.ForEach(c =>
            {
                Vector2 normal;
                FixedArray2<Vector2> points;
                c.GetWorldManifold(out normal, out points);

                _primitiveBatch.DrawLine(
                    Color.Red,
                    c.FixtureA.Body.WorldCenter,
                    Color.Green,
                    c.FixtureB.Body.WorldCenter);
            });
            
        }
        #endregion

        #region Event Handlers
        private bool HandleSensorCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if(fixtureB.UserData is ShipPart sp && sp.IsRoot)
            {
                _contacts.Add(contact);
            }

            return true;
        }
        #endregion
    }
}
