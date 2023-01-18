using Microsoft.Extensions.Configuration;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public partial class Jointable
    {
        public class Joint
        {
            public readonly JointConfiguration Configuration;

            public readonly Entity Entity;

            public Matrix LocalTransformation;

            public Joint(JointConfiguration configuration, Entity entity)
            {
                this.Configuration = configuration;
                this.Entity = entity;

                this.Reset();
            }

            public void Reset()
            {
                this.LocalTransformation = this.Configuration.Transformation;
            }
        }
    }
}
