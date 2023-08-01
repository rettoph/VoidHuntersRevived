using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Collision.Shapes;

namespace VoidHuntersRevived.Domain.Physics
{
    internal sealed class Fixture : IFixture, IDisposable
    {
        private readonly Body _body;
        internal readonly AetherFixture _aether;

        public VhId Id { get; }

        public IBody Body => _body;

        public FixVector2 Centeroid { get; set; }

        public Fixture(VhId id, Body body, Shape shape, Category colissionCategories, Category collidesWith)
        {
            _body = body;
            _aether = _body._aether.CreateFixture(shape);
            _aether.Tag = this;
            _aether.CollisionCategories = colissionCategories;
            _aether.CollidesWith = collidesWith;

            this.Id = id;
        }

        public void Dispose()
        {
            _body._aether.Remove(_aether);
        }
    }
}
