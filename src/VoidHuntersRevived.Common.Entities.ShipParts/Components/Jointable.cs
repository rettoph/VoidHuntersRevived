using MonoGame.Extended.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public sealed partial class Jointable
    {
        public readonly JointableConfiguration Configuration;

        public readonly Entity Entity;
        public readonly Jointable.Joint[] Joints;

        internal Jointable(JointableConfiguration configuration, Entity entity)
        {
            this.Configuration = configuration;
            this.Entity = entity;

            this.Joints = configuration.Joints.Select(x => new Jointable.Joint(x, this)).ToArray();
        }
    }
}
