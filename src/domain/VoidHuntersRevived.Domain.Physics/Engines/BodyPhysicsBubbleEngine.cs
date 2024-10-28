using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.FixedPoint.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    public class BodyPhysicsBubbleEngine(IEntityQueryService entityQueryService, ISpace space) : StrategyEngine, IOnStepEngine, IOnDespawnEngine<Enabled>
    {
        private static readonly Fix64 Two = (Fix64)2;

        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ISpace _space = space;
        private FixRectangle[] _bubbleBuffer = new FixRectangle[8];
        private int _bubbleBufferCount = 0;

        [SequenceGroup<OnStepSequenceGroup>(OnStepSequenceGroup.SyncronizeEntities)]
        public void OnStep(Step step)
        {
            _bubbleBufferCount = 0;
            foreach (var ((bubbles, locations, count), _) in _entityQueryService.QueryEntities<PhysicsBubble, Location>())
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

            foreach (var ((ids, enableds, locations, statuses, count), _) in _entityQueryService.QueryEntities<EntityId, Enabled, Location, EntityStatus>())
            {
                for (int i = 0; i < count; i++)
                {
                    EntityStatus status = statuses[i];
                    if (status.IsSpawned)
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

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, EntityId id, ref Enabled component, in GroupIndex groupIndex)
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
