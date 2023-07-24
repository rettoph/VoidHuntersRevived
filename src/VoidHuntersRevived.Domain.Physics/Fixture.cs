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

namespace VoidHuntersRevived.Domain.Physics
{
    internal sealed class Fixture : IFixture, IDisposable
    {
        private readonly Body _body;
        private readonly AetherFixture _aether;

        public VhId Id { get; }

        public IBody Body => _body;

        public Fixture(Body body, Polygon polygon, Category colissionCategories, Category collidesWith, VhId entityId)
        {
            _body = body;
            _aether = _body._aether.CreateFixture(polygon.ToShape());
            _aether.Tag = this;
            _aether.CollisionCategories = colissionCategories;
            _aether.CollidesWith = collidesWith;

            this.Id = entityId;
        }

        public void Dispose()
        {
            _body._aether.Remove(_aether);
        }
    }
}
