using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics;

namespace VoidHuntersRevived.Domain.Physics
{
    internal sealed class Fixture : IFixture, IDisposable
    {
        private readonly Body _body;
        internal readonly AetherFixture _aether;

        public VhId Id { get; }

        public EntityId EntityId { get; }

        public IBody Body => _body;

        public FixVector2 Centeroid { get; set; }

        public Fixture(VhId id, EntityId entityId, Body body, Shape shape, Category colissionCategories, Category collidesWith)
        {
            _body = body;
            _aether = _body._aether.CreateFixture(shape);
            _aether.Tag = this;
            _aether.CollisionCategories = colissionCategories;
            _aether.CollidesWith = collidesWith;

            this.Id = id;
            this.EntityId = entityId;
        }

        public void Dispose()
        {
            _body._aether.Remove(_aether);
        }
    }
}
