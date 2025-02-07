using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.FixedPoint.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Systems;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using BodyComponent = VoidHuntersRevived.Domain.Physics.Common.Components.Body;

namespace VoidHuntersRevived.Domain.Physics.Systems
{
    public class BodyPhysicsBubbleSystem(IEntityQueryService entityQueryService, ISpace space) : ISceneSystem, IStepSystem, IOnDespawnSystem<Enabled>
    {
        private static readonly Fix64 _two = (Fix64)2;

        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ISpace _space = space;
        private FixRectangle[] _bubbleBuffer = new FixRectangle[8];
        private int _bubbleBufferCount = 0;

        [SequenceGroup<StepSequenceGroupEnum>(StepSequenceGroupEnum.SyncronizeEntities)]
        public void Step(Step step)
        {
            this._bubbleBufferCount = 0;
            foreach (var ((bubbles, bodyComponents, count), _) in this._entityQueryService.QueryEntities<PhysicsBubble, BodyComponent>())
            {
                this.EnsureBubbleBufferCapacity(count);

                for (int i = 0; i < count; i++)
                {
                    PhysicsBubble physicsBubble = bubbles[i];

                    if (physicsBubble.Enabled)
                    {
                        BodyComponent bodyComponent = bodyComponents[i];
                        Fix64 diameter = physicsBubble.Radius * _two;

                        this._bubbleBuffer[this._bubbleBufferCount++] = new FixRectangle()
                        {
                            X = bodyComponent.Transform.Position.X - physicsBubble.Radius,
                            Y = bodyComponent.Transform.Position.Y - physicsBubble.Radius,
                            Width = diameter,
                            Height = diameter
                        };
                    }
                }
            }

            foreach (var ((localIds, enableds, bodyComponents, statuses, count), _) in this._entityQueryService.QueryEntities<EntityLocalId, Enabled, BodyComponent, EntityStatus>())
            {
                for (int i = 0; i < count; i++)
                {
                    EntityStatus status = statuses[i];
                    if (status.IsSpawned)
                    {
                        ref Enabled enabled = ref enableds[i];
                        BodyComponent bodyComponent = bodyComponents[i];

                        bool withinPhysicsBubble = this.WithinPhysicsBubble(bodyComponent);

                        if (enabled.Value == withinPhysicsBubble)
                        { // No change needed
                            continue;
                        }

                        if (enabled.Value == true && withinPhysicsBubble == false)
                        { // disable piece no longer contained within physics bubble
                            this._space.DisableBody(localIds[i]);
                            enabled.Value = false;
                            continue;
                        }

                        if (enabled.Value == false && withinPhysicsBubble == true)
                        { // enable piece now within physics bubble
                            this._space.EnableBody(localIds[i]);
                            enabled.Value = true;
                            continue;
                        }
                    }
                }
            }
        }

        private void EnsureBubbleBufferCapacity(int count)
        {
            int requiredLength = this._bubbleBufferCount + count;
            if (requiredLength < this._bubbleBuffer.Length)
            {
                return;
            }

            Array.Resize<FixRectangle>(ref this._bubbleBuffer, requiredLength);
        }

        private bool WithinPhysicsBubble(BodyComponent bodyComponent)
        {
            for (int i = 0; i < this._bubbleBufferCount; i++)
            {
                if (this._bubbleBuffer[i].Contains(bodyComponent.Position) == false)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Enabled> enabled)
        {
            if (enabled.Component.Value == false)
            {
                return;
            }

            this._space.DisableBody(enabled.LocalId);
            enabled.Component.Value = false;
        }
    }
}