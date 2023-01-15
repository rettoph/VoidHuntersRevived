using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Entities.Configurations
{
    public sealed class RigidConfiguration : IShipPartComponentConfiguration
    {
        private readonly Rigid _component;

        public readonly Shape[] Shapes;

        public RigidConfiguration(params Shape[] shapes)
        {
            _component = new Rigid(this);
            this.Shapes = shapes;
        }

        public void AttachComponentTo(Entity entity)
        {
            entity.Attach<Rigid>(_component);
        }
    }
}
