using Guppy.Common;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    internal sealed class ParallelEntitySystem : BasicSystem,
        ISubscriber<IEvent<CreateEntity>>
    {
        private readonly IParallelService _parallel;
        private World _world;

        public ParallelEntitySystem(IParallelService parallel)
        {
            _parallel = parallel;
            _world = default!;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _world = world;
        }

        public void Process(in IEvent<CreateEntity> message)
        {
            var entity = _world.CreateEntity();

            message.Simulation.AddEntity(message.Data.Key, entity);
        }
    }
}
