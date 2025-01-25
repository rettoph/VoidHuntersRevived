using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common;
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
        private readonly ILogger _logger;
        private readonly IEntityQueryService _entityQueryService;
        private readonly ISpace _space;
        private readonly Queue<IBody> _awakeChangedBodies;

        public BodyAwakeEngine(
            IEntityQueryService entityQueryService,
            ILogger logger,
            ISpace space)
        {
            this._entityQueryService = entityQueryService;
            this._space = space;
            this._logger = logger;
            this._awakeChangedBodies = new Queue<IBody>();

            this._space.OnBodyEnabled += this.HandleBodyEnabled;
            this._space.OnBodyAwakeChanged += this.HandleBodyAwakeChanged;
        }

        [SequenceGroup<OnStepSequenceGroupEnum>(OnStepSequenceGroupEnum.SyncronizeEntities)]
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

            while (this._awakeChangedBodies.TryDequeue(out IBody? body))
            {
                ref Awake awake = ref this._entityQueryService.QueryByEGID<Awake>(body.EntityLocalId.Value, out _, out bool exists);

                if (exists)
                {
                    awake.Value = body.Awake;
                }
                else
                {
                    this._logger.Warning("Awake state changed to {AwakeValue} for body {BodyEntityLocalId}, but entity not found.", body.Awake, body.EntityLocalId);
                }
            }
        }

        private void HandleBodyEnabled(IBody body)
        {
            ref Awake awake = ref this._entityQueryService.QueryByLocalId<Awake>(body.EntityLocalId);
            body.SleepingAllowed = awake.SleepingAllowed;
        }

        private void HandleBodyAwakeChanged(IBody args)
        {
            this._awakeChangedBodies.Enqueue(args);
        }
    }
}