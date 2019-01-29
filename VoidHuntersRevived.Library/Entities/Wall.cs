using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Library.Entities
{
    public class Wall : NetworkedFarseerEntity
    {
        public RectangleF Boundaries { get; private set; }

        public Wall(EntityInfo info, IGame game) : base(info, game)
        {
            this.Enabled = false;
        }
        public Wall(Int64 id, EntityInfo info, IGame game) : base(id, info, game)
        {
            this.Enabled = false;
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

            // Remove any preexisting fixtures
            foreach (var fixture in this.Body.FixtureList)
                this.Body.DestroyFixture(fixture);

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

        #region INetworkEntity Methods
        public override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            om.Write(this.Boundaries.Width);
            om.Write(this.Boundaries.Height);
        }

        public override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            this.Configure(im.ReadSingle(), im.ReadSingle());
        }
        #endregion
    }
}
