using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Physics
{
    public class Space : StrategyEngine, ISpace
    {
        private readonly Dictionary<VhId, Body> _bodies;
        private readonly ILogger _logger;

        internal readonly AetherWorld _aether;

        public int BodyCount => _aether.BodyList.Count;

        public int ContactCount => _aether.ContactCount;

        public event OnEventDelegate<IBody> OnBodyEnabled;
        public event OnEventDelegate<IBody> OnBodyDisabled;
        public event OnEventDelegate<IBody> OnBodyAwakeChanged;

        public Space(ILogger logger, AetherWorld aether)
        {
            _aether = aether;
            _bodies = [];
            _logger = logger;

            _aether.BodyAwakeChanged += this.HandleBodyAwakeChanged;

            this.OnBodyEnabled = null!;
            this.OnBodyDisabled = null!;
            this.OnBodyAwakeChanged = null!;
        }

        public void EnableBody(in EntityId id)
        {
            if (_bodies.TryGetValue(id.VhId, out Body? cached) == false)
            {
                _logger.Verbose("Enabling {Id}", id.VhId);
                Body body = new(this, id);
                _bodies.Add(id.VhId, body);
                this.OnBodyAwakeChanged(body);
                this.OnBodyEnabled(body);
            }
        }

        public void DisableBody(in EntityId id)
        {
            if (_bodies.Remove(id.VhId, out var body))
            {
                _logger.Verbose("Disabling {Id}", id.VhId);
                body!.Dispose();
                this.OnBodyDisabled?.Invoke(body);
            }
        }

        public IBody GetBody(in EntityId id)
        {
            return _bodies[id.VhId];
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
            if (_bodies.TryGetValue(id.VhId, out Body? instance))
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