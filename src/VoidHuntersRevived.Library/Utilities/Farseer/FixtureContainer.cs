using System;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Extensions.Aether;

namespace VoidHuntersRevived.Library.Utilities.Farseer
{
    public sealed class FixtureContainer
    {
        private List<Fixture> _fixtures;

        public readonly BodyEntity Root;
        public readonly BodyEntity Owner;
        public readonly Shape Shape;

        public IReadOnlyList<Fixture> List => _fixtures;

        public FixtureContainer(BodyEntity root, BodyEntity owner, Shape shape)
        {
            _fixtures = new List<Fixture>(2);

            this.Root = root;
            this.Owner = owner;
            this.Shape = shape;
        }

        internal void Attach(Body body)
        {
            var fixture = body.CreateFixture(this.Shape);
            fixture.Tag = this.Owner;

            // Setup default Fixture properties...
            fixture.CollisionCategories = this.Root.CollisionCategories;
            fixture.CollidesWith = this.Root.CollidesWith;

            _fixtures.Add(fixture);
        }

        public void Destroy()
        {
            _fixtures.ForEach(f => f.TryRemove());
        }
    }
}
