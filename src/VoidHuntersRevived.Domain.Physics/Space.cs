using System.Runtime.CompilerServices;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Physics;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Physics
{
    public class Space : ISpace
    {
        private readonly Dictionary<uint, Body> _bodies;

        internal readonly AetherWorld _aether;

        public event OnEventDelegate<IBody> OnBodyAwakeChanged;

        public Space()
        {
            _aether = new AetherWorld(AetherVector2.Zero);
            _bodies = new Dictionary<uint, Body>();

            _aether.BodyAwakeChanged += this.HandleBodyAwakeChanged;
        }

        public IBody GetOrCreateBody(in EntityId id)
        {
            if(_bodies.TryGetValue(id.EGID.entityID, out Body? cached))
            {
                return cached;

            }
            
            Body body = new Body(this, id);
            _bodies.Add(id.EGID.entityID, body);
            this.OnBodyAwakeChanged(body);

            return body;
        }

        public void DestroyBody(in EntityId id)
        {
            _bodies.Remove(id.EGID.entityID, out var body);
            body!.Dispose();
        }

        public IBody GetBody(in EntityId id)
        {
            return _bodies[id.EGID.entityID];
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

        public bool TryGetBody(in EntityId id, [MaybeNullWhen(false)] out IBody body)
        {
            if(_bodies.TryGetValue(id.EGID.entityID, out Body? instance))
            {
                body = instance;
                return true;
            }

            body = null;
            return false;
        }

        private void HandleBodyAwakeChanged(AetherWorld sender, AetherBody body)
        {
            this.OnBodyAwakeChanged((IBody)body.Tag);
        }
    }
}