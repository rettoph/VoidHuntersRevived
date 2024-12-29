using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    public sealed class BodyLocationEngine : StrategyEngine, IOnStepEngine
    {
        private readonly IEntityQueryService _entityQueryService;
        private readonly ISpace _space;

        public BodyLocationEngine(
            IEntityQueryService entityQueryService,
            ISpace space)
        {
            _entityQueryService = entityQueryService;
            _space = space;

            _space.OnBodyEnabled += this.HandleBodyEnabled;
        }


        [SequenceGroup<OnStepSequenceGroup>(OnStepSequenceGroup.SyncronizeEntities)]
        public void OnStep(Step step)
        {
            foreach (var ((localIds, locations, enableds, awakes, count), _) in _entityQueryService.QueryEntities<EntityLocalId, BodyLocation, Enabled, Awake>())
            {
                for (int i = 0; i < count; i++)
                {
                    if (enableds[i] == false || awakes[i] == false)
                    {
                        continue;
                    }

                    EntityLocalId localId = localIds[i];
                    IBody body = _space.GetBody(localId);

                    ref BodyLocation location = ref locations[i];
                    location.SetRotationTransform(body.Rotation, body.Transform);
                }
            }
        }

        private void HandleBodyEnabled(IBody body)
        {
            ref BodyLocation location = ref _entityQueryService.QueryByLocalId<BodyLocation>(body.EntityLocalId);
            body.SetTransform(location.Transform);
        }
    }
}
