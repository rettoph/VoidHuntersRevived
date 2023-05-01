using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Events
{
    public class CreateEntity : ISimulationEventData
    {
        public ParallelKey Key { get; }

        public readonly ParallelKey EntityKey;

        public CreateEntity(ParallelKey entity)
        {
            this.EntityKey = entity;
        }
    }
}
