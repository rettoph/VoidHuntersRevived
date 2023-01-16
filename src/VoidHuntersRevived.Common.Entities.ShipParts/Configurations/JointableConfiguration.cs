using MonoGame.Extended.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Configurations
{
    public sealed class JointableConfiguration : IShipPartComponentConfiguration
    {
        public readonly JointConfiguration[] Joints;

        public JointableConfiguration(params JointConfiguration[] joints)
        {
            this.Joints = joints;
        }

        public void AttachComponentTo(Entity entity)
        {
            entity.Attach(new Jointable(this, entity));
        }
    }
}
