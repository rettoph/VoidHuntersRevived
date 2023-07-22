using System.Runtime.CompilerServices;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Physics;
using System.Diagnostics.CodeAnalysis;

namespace VoidHuntersRevived.Domain.Physics
{
    public class Space : ISpace
    {
        private readonly Dictionary<VhId, Body> _bodies;

        internal readonly AetherWorld _aether;

        public Space()
        {
            _aether = new AetherWorld(AetherVector2.Zero);
            _bodies = new Dictionary<VhId, Body>();
        }

        public IBody GetOrCreateBody(in VhId id)
        {
            if(_bodies.TryGetValue(id, out Body? cached))
            {
                return cached;

            }
            
            Body body = new Body(this, id);
            _bodies.Add(id, body);

            return body;
        }

        public void DestroyBody(in VhId id)
        {
            _bodies.Remove(id, out var body);
            body!.Dispose();
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

        public IEnumerable<IBody> AllBodies()
        {
            return _bodies.Values;
        }

        public bool TryGetBody(in VhId id, [MaybeNullWhen(false)] out IBody body)
        {
            if(_bodies.TryGetValue(id, out Body? instance))
            {
                body = instance;
                return true;
            }

            body = null;
            return false;
        }
    }
}