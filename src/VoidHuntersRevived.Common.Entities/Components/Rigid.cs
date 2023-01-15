using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Configurations;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public sealed class Rigid
    {
        public readonly RigidConfiguration Configuration;

        public Rigid(RigidConfiguration configuration)
        {
            this.Configuration = configuration;
        }
    }
}
