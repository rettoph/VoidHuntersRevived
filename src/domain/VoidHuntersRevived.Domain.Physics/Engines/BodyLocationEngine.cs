using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using BodyComponent = VoidHuntersRevived.Domain.Physics.Common.Components.Body;
using FixtureComponent = VoidHuntersRevived.Domain.Physics.Common.Components.Fixture;

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
            this._entityQueryService = entityQueryService;
            this._space = space;

            this._space.OnBodyEnabled += this.HandleBodyEnabled;
        }


        [SequenceGroup<OnStepSequenceGroupEnum>(OnStepSequenceGroupEnum.SyncronizeEntities)]
        public void OnStep(Step step)
        {
            foreach (var ((localIds, bodyComponents, enableds, awakes, count), _) in this._entityQueryService.QueryEntities<EntityLocalId, BodyComponent, Enabled, Awake>())
            {
                for (int i = 0; i < count; i++)
                {
                    if (enableds[i] == false || awakes[i] == false)
                    {
                        continue;
                    }

                    EntityLocalId localId = localIds[i];
                    IBody bodyInstance = this._space.GetBody(localId);

                    ref BodyComponent bodyComponent = ref bodyComponents[i];
                    bodyComponent.SetRotationTransform(bodyInstance.Rotation, bodyInstance.Transform);

                    ref var fixtureFilter = ref this._entityQueryService.GetFilter(bodyComponent.FixtureFilterId);
                    foreach (var (indices, group) in fixtureFilter)
                    {
                        var (fixtures, _) = this._entityQueryService.QueryEntities<FixtureComponent>(group);

                        for (int j = 0; j < indices.count; j++)
                        {
                            ref FixtureComponent fixture = ref fixtures[indices[j]];
                            fixture.SetBodyTransform(bodyComponent.Transform);
                        }
                    }
                }
            }
        }

        private void HandleBodyEnabled(IBody bodyInstance)
        {
            ref BodyComponent bodyComponent = ref this._entityQueryService.QueryByLocalId<Common.Components.Body>(bodyInstance.EntityLocalId);
            bodyInstance.SetTransform(bodyComponent.Transform);
        }
    }
}