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
        private readonly IBodyFactory _factory;
        private readonly Dictionary<VhId, Body> _bodies;

        public Space(IBodyFactory factory)
        {
            _aether = new AetherWorld(AetherVector2.Zero);
            _bodies = new Dictionary<VhId, Body>();
            _factory = factory;
        }

        public IBody GetOrCreateBody(in VhId id)
        {
            if(_bodies.TryGetValue(id, out Body? cached))
            {
                return cached;

            }
            
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

        public void RemoveBody(in VhId id)
        {
            this.RemoveBody(this.GetBody(id));
        }

        internal void Add(Body body)
        {
            _bodies.Add(body.Id, body);
            body.AddToWorld(_aether);
            body.Space = this;
        }

        internal void Remove(Body body)
        {
            if (!_bodies.Remove(body.Id))
            {
                return;
            }

            body.Space = null;
            body.RemoveFromWorld(_aether);
        }

        public IBody GetBody(in VhId id)
        {
            return _bodies[id];
        }

        public void QueryAABB(QueryReportFixtureDelegate callback, ref AABB aabb)
        {
            _aether.QueryAABB(aetherFixture =>
            {
                return callback((Fixture)aetherFixture.Tag);
            }, ref Unsafe.As<AABB, AetherAABB>(ref aabb));
        }

        public void Step(Step step)
        {
            _aether.Step(step.ElapsedTime);
        }
    }
}