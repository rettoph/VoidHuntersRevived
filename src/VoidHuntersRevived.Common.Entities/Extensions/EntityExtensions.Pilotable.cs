using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Components;
using tainicom.Aether.Physics2D.Dynamics;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static partial class EntityExtensions
    {
        public static Entity MakePilotable(this Entity entity)
        {
            entity.Attach(new Pilotable(entity.Id));

            return entity;
        }

        public static bool IsPilotable(this Entity entity)
        {
            return entity.Has<Pilotable>();
        }
    }
}
