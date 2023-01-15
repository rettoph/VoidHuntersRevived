using MonoGame.Extended.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Configurations;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public sealed partial class Linkable
    {
        public readonly LinkableConfiguration Configuration;

        public readonly Entity Entity;
        public readonly Linkable.Joint[] Joints;

        internal Linkable(LinkableConfiguration configuration, Entity entity)
        {
            this.Configuration = configuration;
            this.Entity = entity;

            this.Joints = configuration.Joints.Select(x => new Linkable.Joint(x, this)).ToArray();
        }
    }
}
