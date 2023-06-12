using System.Runtime.CompilerServices;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Factories;

namespace VoidHuntersRevived.Domain.Physics
{
    public class Space : ISpace
    {
        private readonly AetherWorld _aether;
        private readonly HashSet<Body> _bodies;
        private readonly IBodyFactory _factory;

        public Space(IBodyFactory factory)
        {
            _aether = new AetherWorld(AetherVector2.Zero);
            _bodies = new HashSet<Body>();
            _factory = factory;
        }

        public IBody CreateBody(Guid id)
        {
            IBody body = _factory.Create(id);
            this.AddBody(body);

            return body;
        }

        public void AddBody(IBody body)
        {
            if(body is Body casted)
            {
                this.Add(casted);
            }
        }

        public void RemoveBody(IBody body)
        {
            if (body is Body casted)
            {
                this.Remove(casted);
            }
        }

        internal void Add(Body body)
        {
            _bodies.Add(body);
            body.AddToWorld(_aether);
            body.Space = this;
        }

        internal void Remove(Body body)
        {
            if (!_bodies.Remove(body))
            {
                return;
            }

            body.Space = null;
            body.RemoveFromWorld(_aether);
        }

        public void QueryAABB(QueryReportFixtureDelegate callback, ref AABB aabb)
        {
            _aether.QueryAABB(aetherFixture =>
            {
                return callback((Fixture)aetherFixture.Tag);
            }, ref Unsafe.As<AABB, AetherAABB>(ref aabb));
        }

        public void Step(TimeSpan elapsedGameTime)
        {
            _aether.Step((Fix64)elapsedGameTime.TotalSeconds);
        }
    }
}