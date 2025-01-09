using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Physics.Common;

namespace VoidHuntersRevived.Domain.Physics
{
    internal sealed class Fixture : IFixture, IDisposable
    {
        private readonly Body _body;
        internal readonly AetherFixture aether;

        public FixtureId Id { get; }

        public IBody Body => this._body;

        public FixVector2 Centeroid { get; set; }

        public Fixture(FixtureId id, Body body, Shape shape, Category colissionCategories, Category collidesWith)
        {
            this._body = body;
            this.aether = this._body.aether.CreateFixture(shape);
            this.aether.Tag = this;
            this.aether.CollisionCategories = colissionCategories;
            this.aether.CollidesWith = collidesWith;

            this.Id = id;
        }

        public void Dispose()
        {
            this._body.aether.Remove(this.aether);
        }
    }
}