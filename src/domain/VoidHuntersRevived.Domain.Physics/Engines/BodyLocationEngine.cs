using Guppy.Attributes;
using Guppy.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    [Sequence<StepSequence>(StepSequence.PostResourceManagerUpdate)]
    internal sealed class BodyLocationEngine : BasicEngine, IStepEngine<Step>
    {
        private readonly IEntityService _entities;
        private readonly ISpace _space;

        public BodyLocationEngine(IEntityService entities, ISpace space)
        {
            _entities = entities;
            _space = space;

            _space.OnBodyEnabled += this.HandleBodyEnabled;
        }

        public string name { get; } = nameof(BodyLocationEngine);

        public void Step(in Step _param)
        {
            foreach (var ((ids, locations, enableds, awakes, count), _) in _entities.QueryEntities<EntityId, Location, Enabled, Awake>())
            {
                for (int i = 0; i < count; i++)
                {
                    if (enableds[i] == false || awakes[i] == false)
                    {
                        continue;
                    }

                    EntityId id = ids[i];
                    IBody body = _space.GetBody(id);

                    ref Location location = ref locations[i];
                    location.Position = body.Position;
                    location.Rotation = body.Rotation;
                }
            }
        }

        private void HandleBodyEnabled(IBody body)
        {
            ref Location location = ref _entities.QueryById<Location>(body.Id);
            body.SetTransform(location.Position, location.Rotation);
        }
    }
}
