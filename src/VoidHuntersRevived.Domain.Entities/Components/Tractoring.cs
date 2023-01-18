using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    public sealed class Tractoring
    {
        public Entity Tractorable;

        public Tractoring(Entity tractorable)
        {
            this.Tractorable = tractorable;
        }
    }
}
