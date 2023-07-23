using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Dynamics;

namespace VoidHuntersRevived.Domain.Physics
{
    internal sealed class Fixture : IFixture, IDisposable
    {
        private readonly Body _body;
        private readonly AetherFixture _aether;

        public VhId Id { get; }

        public IBody Body => _body;

        public Fixture(Body body, Polygon polygon, VhId entityId)
        {
            _body = body;
            _aether = _body._aether.CreateFixture(polygon.ToShape());
            _aether.Tag = this;
            //_aether.CollisionGroup = -1;

            this.Id = entityId;
        }

        public void Dispose()
        {
            _body._aether.Remove(_aether);
        }
    }
}
