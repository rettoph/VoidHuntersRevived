using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Utilities.Farseer
{
    public sealed class FixtureContainer
    {
        private List<Fixture> _fixtures;

        public readonly BodyEntity Root;
        public readonly BodyEntity Owner;
        public readonly PolygonShape Shape;

        public FixtureContainer(BodyEntity root, BodyEntity owner, PolygonShape shape)
        {
            _fixtures = new List<Fixture>(2);

            this.Root = root;
            this.Owner = owner;
            this.Shape = shape;
        }

        internal void Attach(Body body)
        {
            var fixture = body.CreateFixture(this.Shape, this.Owner);

            // Setup default Fixture properties...
            fixture.CollisionCategories = this.Root.CollisionCategories;
            fixture.CollidesWith = this.Root.CollidesWith;
            fixture.IgnoreCCDWith = this.Root.IgnoreCCDWith;

            _fixtures.Add(fixture);
        }

        public void Destroy()
        {
            this.Root.DestroyFixture(this);
        }

        internal void Destroy(Boolean external)
        {
            if(external)
                _fixtures.ForEach(f => f.Dispose());
        }
    }
}
