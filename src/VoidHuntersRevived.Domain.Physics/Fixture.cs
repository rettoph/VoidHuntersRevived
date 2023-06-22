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
    internal sealed class Fixture : IFixture
    {
        private readonly AetherFixture _aether;

        public VhId Id { get; }

        public IBody Body { get; }

        public Fixture(IBody body, Polygon polygon, VhId entityId)
        {
            _aether = new AetherFixture(polygon.ToShape());
            _aether.Tag = this;

            this.Id = entityId;
            this.Body = body;
        }

        internal void AddToBody(AetherBody body)
        {
            body.Add(_aether);
        }

        internal void RemoveFromBody(AetherBody body)
        {
            body.Remove(_aether);
        }
    }
}
