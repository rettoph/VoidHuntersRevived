using System;
using System.Collections.Generic;
using System.Drawing;
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
        public RectangleF Boundaries { get; private set; }

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

            this.Boundaries = new RectangleF(-halfWidth, -halfHeight, width, height);
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.Body.BodyType = BodyType.Kinematic;
            this.Body.IsBullet = true;
            this.Body.IgnoreCCD = false;
            this.Body.Restitution = 0f;
            this.Body.Friction = 0f;

            this.Body.CollisionCategories = Category.Cat1;
        }
    }
}
