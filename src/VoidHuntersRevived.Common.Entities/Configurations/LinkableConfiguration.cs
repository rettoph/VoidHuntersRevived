using MonoGame.Extended.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Entities.Configurations
{
    public sealed class LinkableConfiguration : IShipPartComponentConfiguration
    {
        public readonly JointConfiguration[] Joints;

        public LinkableConfiguration(params JointConfiguration[] joints)
        {
            this.Joints = joints;
        }

        public void AttachComponentTo(Entity entity)
        {
            entity.Attach(new Linkable(this, entity));
        }
    }
}
