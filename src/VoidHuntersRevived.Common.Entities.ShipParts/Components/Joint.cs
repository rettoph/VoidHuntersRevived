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
    public class Joint
    {
        public readonly JointConfiguration Configuration;
        public readonly Node Node;
        public readonly int Index;

        public Link? Link;

        public Matrix LocalTransformation;
        public Vector2 LocalPosition => Vector2.Transform(Vector2.Zero, this.LocalTransformation);

        public Joint(JointConfiguration configuration, Node node, int index)
        {
            this.Configuration = configuration;
            this.Node = node;
            this.Index = index;

            this.Reset();
        }

        public void Reset()
        {
            this.LocalTransformation = this.Configuration.Transformation;
        }
    }
}
