using Guppy.Attributes;
using Guppy.Common.Attributes;
using Serilog;
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
    internal sealed class BodyAwakeEngine : BasicEngine, IStepEngine<Step>
    {
        public string name { get; } = nameof(BodyAwakeEngine);

        private readonly ILogger _logger;
        private readonly IEntityService _entities;
        private readonly Space _space;
        private readonly Queue<IBody> _awakeChangedBodies;

        public BodyAwakeEngine(IEntityService entities, ILogger logger, Space space)
        {
            _entities = entities;
            _space = space;
            _logger = logger;
            _awakeChangedBodies = new Queue<IBody>();

            _space.OnBodyEnabled += this.HandleBodyEnabled;
            _space.OnBodyAwakeChanged += this.HandleBodyAwakeChanged;
        }

        public void Step(in Step _param)
        {
            //foreach (var ((ids, awakes, count), _) in _entities.QueryEntities<EntityId, Awake>())
            //{
            //    for (int i = 0; i < count; i++)
            //    {
            //        IBody body = _space.GetBody(ids[i]);
            //
            //        ref Awake awake = ref awakes[i];
            //        awake.Value = body.Awake;
            //    }
            //}

            while (_awakeChangedBodies.TryDequeue(out IBody? body))
            {
                ref Awake awake = ref _entities.QueryById<Awake>(body.Id, out _, out bool exists);

                if (exists)
                {
                    awake.Value = body.Awake;
                }
                else
                {
                    _logger.Warning("{ClassName}::{MethodName} - Awake state changed to {AwakeValue} for body {BodyId}, but entity not found.", nameof(BodyAwakeEngine), nameof(Step), body.Awake, body.Id.VhId);
                }
            }
        }

        private void HandleBodyEnabled(IBody body)
        {
            ref Awake awake = ref _entities.QueryById<Awake>(body.Id);
            body.SleepingAllowed = awake.SleepingAllowed;
        }

        private void HandleBodyAwakeChanged(IBody args)
        {
            _awakeChangedBodies.Enqueue(args);
        }
    }
}
