using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Physics.Common;

namespace VoidHuntersRevived.Domain.Physics
{
    internal sealed class Fixture : IFixture, IDisposable
    {
        private readonly Body _body;
        internal readonly AetherFixture _aether;

        public FixtureId Id { get; }

        public IBody Body => this._body;

        public FixVector2 Centeroid { get; set; }

        public Fixture(FixtureId id, Body body, Shape shape, Category colissionCategories, Category collidesWith)
        {
            this._body = body;
            this._aether = this._body._aether.CreateFixture(shape);
            this._aether.Tag = this;
            this._aether.CollisionCategories = colissionCategories;
            this._aether.CollidesWith = collidesWith;

            this.Id = id;
        }

        public void Dispose()
        {
            this._body._aether.Remove(this._aether);
        }
    }
}