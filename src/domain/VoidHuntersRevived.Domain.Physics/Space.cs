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
        private readonly Dictionary<EntityLocalId, Body> _bodies;
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

        public void EnableBody(in EntityLocalId entityLocalId)
        {
            if (_bodies.TryGetValue(entityLocalId, out Body? cached) == false)
            {
                _logger.Verbose("Enabling BodyEntityLocalId {BodyEntityLocalId}", entityLocalId);
                Body body = new(entityLocalId, this);
                _bodies.Add(entityLocalId, body);
                this.OnBodyAwakeChanged(body);
                this.OnBodyEnabled(body);
            }
        }

        public void DisableBody(in EntityLocalId entityLocalId)
        {
            if (_bodies.Remove(entityLocalId, out var body))
            {
                _logger.Verbose("Disabling {BodyEntityLocalId}", entityLocalId);
                body!.Dispose();
                this.OnBodyDisabled?.Invoke(body);
            }
        }

        public IBody GetBody(in EntityLocalId entityLocalId)
        {
            return _bodies[entityLocalId];
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

        public bool TryGetBody(in EntityLocalId entityLocalId, [MaybeNullWhen(false)] out IBody body)
        {
            if (_bodies.TryGetValue(entityLocalId, out Body? instance))
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