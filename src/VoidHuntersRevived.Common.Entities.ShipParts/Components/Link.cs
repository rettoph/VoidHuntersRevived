using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public class Link
    {
        public readonly Joint Child;
        public readonly Joint Parent;

        public Matrix LocalTransformation => CalculateLocalTransformation(Parent, Child);

        public Link(Joint childJoint, Joint parentJoint)
        {
            this.Child = childJoint;
            this.Parent = parentJoint;
        }

        public static Matrix CalculateLocalTransformation(Joint parent, Joint child)
        {
            Matrix transformation = Matrix.Invert(child.Configuration.Transformation);
            transformation *= Matrix.CreateRotationZ(MathHelper.Pi);
            transformation *= parent.LocalTransformation;

            return transformation;
        }
    }
}
