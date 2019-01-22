using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;

namespace VoidHuntersRevived.Library.Entities
{
    public class Wall : FarseerEntity
    {
        public Wall(EntityInfo info, IGame game) : base(info, game)
        {
        }

        /// <summary>
        /// Create a new wall configured to the inputed width and height
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Configure(Single width, Single height)
        {
            var halfWidth = width / 2;
            var halfHeight = height / 2;

            this.Body.CreateFixture(new EdgeShape(new Vector2(halfWidth, -halfHeight), new Vector2(-halfWidth, -halfHeight)));
            this.Body.CreateFixture(new EdgeShape(new Vector2(halfWidth, -halfHeight), new Vector2(halfWidth, halfHeight)));
            this.Body.CreateFixture(new EdgeShape(new Vector2(-halfWidth, halfHeight), new Vector2(halfWidth, halfHeight)));
            this.Body.CreateFixture(new EdgeShape(new Vector2(-halfWidth, halfHeight), new Vector2(-halfWidth, -halfHeight)));
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        protected override void Boot()
        {
            // throw new NotImplementedException();
        }

        protected override void PreInitialize()
        {
            // throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PostInitialize()
        {
            // throw new NotImplementedException();
        }

        protected override void HandleAddedToScene(object sender, ISceneObject e)
        {
            base.HandleAddedToScene(sender, e);

            this.Body.BodyType = BodyType.Kinematic;
            this.Body.IsBullet = true;
            this.Body.IgnoreCCD = false;
            this.Body.Restitution = 0f;
            this.Body.Friction = 0f;

            this.Body.CollisionCategories = Category.Cat1;
        }
    }
}
