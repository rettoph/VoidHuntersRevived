using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Ships.Components
{
    public struct Tractorable : IEntityComponent
    {
        public EntityId TractorBeamEmitter { get; init; }
    }
}
