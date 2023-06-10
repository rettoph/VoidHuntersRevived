using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS.Systems;

namespace VoidHuntersRevived.Common.Physics.Systems
{
    public abstract class PhysicsSystem : BasicSystem, IPhysicsSystem
    {
        public ISpace Space { get; private set; } = null!;

        public virtual void Initialize(ISpace space)
        {
            this.Space = space;
        }
    }
}
