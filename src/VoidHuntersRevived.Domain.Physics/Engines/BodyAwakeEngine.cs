using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.MonoGame.Common.Enums;
using Serilog;
using tainicom.Aether.Physics2D.Dynamics;

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

            while(_awakeChangedBodies.TryDequeue(out IBody? body))
            {
                ref Awake awake = ref _entities.QueryById<Awake>(body.Id, out bool exists);

                if(exists)
                {
                    awake.Value = body.Awake;
                }
                else
                {
                    _logger.Warning("{ClassName}::{MethodName} - Awake state changed to {AwakeValue} for body {BodyId}, but entity not found.", nameof(BodyAwakeEngine), nameof(Step), body.Awake, body.Id);
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
