using Guppy.Attributes;
using Guppy.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    [Sequence<StepSequence>(StepSequence.PostResourceManagerUpdate)]
    internal class BodyPhysicsBubbleEngine : BasicEngine, IStepEngine<Step>, IOnDespawnEngine<Enabled>
    {
        private static readonly Fix64 Two = (Fix64)2;

        private readonly IEntityService _entities;
        private readonly ISpace _space;
        private FixRectangle[] _bubbleBuffer;
        private int _bubbleBufferCount;

        public string name { get; } = nameof(BodyPhysicsBubbleEngine);

        public BodyPhysicsBubbleEngine(IEntityService entities, ISpace space)
        {
            _entities = entities;
            _space = space;
            _bubbleBuffer = new FixRectangle[8];
            _bubbleBufferCount = 0;
        }

        public void Step(in Step param)
        {
            _bubbleBufferCount = 0;
            foreach (var ((bubbles, locations, count), _) in _entities.QueryEntities<PhysicsBubble, Location>())
            {
                this.EnsureBubbleBufferCapacity(count);

                for (int i = 0; i < count; i++)
                {
                    PhysicsBubble physicsBubble = bubbles[i];

                    if (physicsBubble.Enabled)
                    {
                        Location location = locations[i];
                        Fix64 diameter = physicsBubble.Radius * Two;

                        _bubbleBuffer[_bubbleBufferCount++] = new FixRectangle()
                        {
                            X = location.Position.X - physicsBubble.Radius,
                            Y = location.Position.Y - physicsBubble.Radius,
                            Width = diameter,
                            Height = diameter
                        };
                    }
                }
            }

            foreach (var ((ids, enableds, locations, statuses, count), _) in _entities.QueryEntities<EntityId, Enabled, Location, EntityStatus>())
            {
                for (int i = 0; i < count; i++)
                {
                    if (statuses[i].IsSpawned)
                    {
                        ref Enabled enabled = ref enableds[i];
                        Location location = locations[i];

                        bool withinPhysicsBubble = this.WithinPhysicsBubble(location);

                        if (enabled.Value == withinPhysicsBubble)
                        { // No change needed
                            continue;
                        }

                        if (enabled.Value == true && withinPhysicsBubble == false)
                        { // disable piece no longer contained within physics bubble
                            _space.DisableBody(ids[i]);
                            enabled.Value = false;
                            continue;
                        }

                        if (enabled.Value == false && withinPhysicsBubble == true)
                        { // enable piece now within physics bubble
                            _space.EnableBody(ids[i]);
                            enabled.Value = true;
                            continue;
                        }
                    }
                }
            }
        }

        private void EnsureBubbleBufferCapacity(int count)
        {
            int requiredLength = _bubbleBufferCount + count;
            if (requiredLength < _bubbleBuffer.Length)
            {
                return;
            }

            Array.Resize<FixRectangle>(ref _bubbleBuffer, requiredLength);
        }

        private bool WithinPhysicsBubble(Location location)
        {
            for (int i = 0; i < _bubbleBufferCount; i++)
            {
                if (_bubbleBuffer[i].Contains(location.Position) == false)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public void OnDespawn(VhId sourceEventId, EntityId id, ref Enabled component, in GroupIndex groupIndex)
        {
            if (component.Value == false)
            {
                return;
            }

            _space.DisableBody(id);
            component.Value = false;
        }
    }
}
