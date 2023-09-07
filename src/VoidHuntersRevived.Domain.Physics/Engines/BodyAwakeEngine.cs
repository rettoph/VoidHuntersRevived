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
using VoidHuntersRevived.Common.Simulations.Enums;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    [Sequence<StepSequence>(StepSequence.Cleanup)]
    internal sealed class BodyAwakeEngine : BasicEngine, IStepEngine<Step>
    {
        public string name { get; } = nameof(BodyAwakeEngine);

        private readonly IEntityService _entities;
        private readonly Space _space;
        private readonly Queue<IBody> _awakeChangedBodies;

        public BodyAwakeEngine(IEntityService entities, Space space)
        {
            _entities = entities;
            _space = space;
            _awakeChangedBodies = new Queue<IBody>();

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
                ref Awake awake = ref _entities.QueryById<Awake>(body.Id);
                awake.Value = body.Awake;
            }
        }

        private void HandleBodyAwakeChanged(IBody args)
        {
            _awakeChangedBodies.Enqueue(args);
        }
    }
}
