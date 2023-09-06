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

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    internal sealed class BodyAwakeEngine : BasicEngine, IStepEngine<Step>
    {
        public string name { get; } = nameof(BodyAwakeEngine);

        private readonly IEntityService _entities;
        private readonly ISpace _space;

        public BodyAwakeEngine(IEntityService entities, ISpace space)
        {
            _entities = entities;
            _space = space;
        }

        public void Step(in Step _param)
        {
            foreach (var ((ids, awakes, count), _) in _entities.QueryEntities<EntityId, Awake>())
            {
                for (int i = 0; i < count; i++)
                {
                    IBody body = _space.GetBody(ids[i].VhId);

                    ref Awake awake = ref awakes[i];
                    awake.Value = body.Awake;
                }
            }
        }
    }
}
