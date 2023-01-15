using Microsoft.Extensions.Configuration;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Configurations;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public partial class Linkable
    {
        public class Joint
        {
            public readonly JointConfiguration Configuration;

            public readonly Linkable Linkable;

            public Matrix LocalTransformation;

            public Joint(JointConfiguration configuration, Linkable linkable)
            {
                this.Configuration = configuration;
                this.Linkable = linkable;

                this.Reset();
            }

            public void Reset()
            {
                this.LocalTransformation = this.Configuration.Transformation;
            }
        }
    }
}
