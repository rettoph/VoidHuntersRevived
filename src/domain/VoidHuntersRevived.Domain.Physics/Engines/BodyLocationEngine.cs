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
            foreach (var ((localIds, locations, enableds, awakes, count), _) in _entityQueryService.QueryEntities<EntityLocalId, Location, Enabled, Awake>())
            {
                for (int i = 0; i < count; i++)
                {
                    if (enableds[i] == false || awakes[i] == false)
                    {
                        continue;
                    }

                    EntityLocalId localId = localIds[i];
                    IBody body = _space.GetBody(localId);

                    ref Location location = ref locations[i];
                    location.Position = body.Position;
                    location.Rotation = body.Rotation;
                }
            }
        }

        private void HandleBodyEnabled(IBody body)
        {
            ref Location location = ref _entityQueryService.QueryByLocalId<Location>(body.EntityLocalId);
            body.SetTransform(location.Position, location.Rotation);
        }
    }
}
