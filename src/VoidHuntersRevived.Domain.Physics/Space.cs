﻿using System.Runtime.CompilerServices;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Physics;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Entities;
using Serilog;

namespace VoidHuntersRevived.Domain.Physics
{
    public class Space : ISpace
    {
        private readonly Dictionary<VhId, Body> _bodies;
        private readonly ILogger _logger;

        internal readonly AetherWorld _aether;

        public event OnEventDelegate<IBody> OnBodyEnabled;
        public event OnEventDelegate<IBody> OnBodyDisabled;
        public event OnEventDelegate<IBody> OnBodyAwakeChanged;

        public Space(ILogger logger)
        {
            _aether = new AetherWorld(AetherVector2.Zero);
            _bodies = new Dictionary<VhId, Body>();
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
                _logger.Debug("{ClassName}::{MethodName} - Enabling {Id}", nameof(Space), nameof(EnableBody), id.VhId);
                Body body = new Body(this, id);
                _bodies.Add(id.VhId, body);
                this.OnBodyAwakeChanged(body);
                this.OnBodyEnabled(body);
            }
        }

        public void DisableBody(in EntityId id)
        {
            if(_bodies.Remove(id.VhId, out var body))
            {
                _logger.Debug("{ClassName}::{MethodName} - Disabling {Id}", nameof(Space), nameof(EnableBody), id.VhId);
                body!.Dispose();
                this.OnBodyDisabled?.Invoke(body);
            }
        }

        public IBody GetBody(in EntityId id)
        {
            return _bodies[id.VhId];
        }

        public int BodyCount()
        {
            return _bodies.Count;
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
            if(_bodies.TryGetValue(id.VhId, out Body? instance))
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