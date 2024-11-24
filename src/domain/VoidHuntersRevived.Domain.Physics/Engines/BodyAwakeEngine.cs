using Guppy.Core.Common.Attributes;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    public sealed class BodyAwakeEngine : StrategyEngine, IOnStepEngine
    {
        public string name { get; } = nameof(BodyAwakeEngine);

        private readonly ILogger _logger;
        private readonly IEntityQueryService _entityQueryService;
        private readonly ISpace _space;
        private readonly Queue<IBody> _awakeChangedBodies;

        public BodyAwakeEngine(
            IEntityQueryService entityQueryService,
            ILogger logger,
            ISpace space)
        {
            _entityQueryService = entityQueryService;
            _space = space;
            _logger = logger;
            _awakeChangedBodies = new Queue<IBody>();

            _space.OnBodyEnabled += this.HandleBodyEnabled;
            _space.OnBodyAwakeChanged += this.HandleBodyAwakeChanged;
        }

        [SequenceGroup<OnStepSequenceGroup>(OnStepSequenceGroup.SyncronizeEntities)]
        public void OnStep(Step step)
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
                ref Awake awake = ref _entityQueryService.QueryById<Awake>(body.Id, out _, out bool exists);

                if (exists)
                {
                    awake.Value = body.Awake;
                }
                else
                {
                    _logger.Warning("Awake state changed to {AwakeValue} for body {BodyId}, but entity not found.", body.Awake, body.Id.VhId);
                }
            }
        }

        private void HandleBodyEnabled(IBody body)
        {
            ref Awake awake = ref _entityQueryService.QueryById<Awake>(body.Id);
            body.SleepingAllowed = awake.SleepingAllowed;
        }

        private void HandleBodyAwakeChanged(IBody args)
        {
            _awakeChangedBodies.Enqueue(args);
        }
    }
}
