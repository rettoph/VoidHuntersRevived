using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    /// <summary>
    /// Can be thought of as the "male"
    /// connection
    /// </summary>
    public sealed class Jointing
    {
        public readonly Jointable.Joint Joint;
        public readonly Jointable.Joint Parent;
        public readonly Matrix LocalTransformation;

        public Jointing(
            Jointable.Joint joint,
            Jointable.Joint parent)
        {
            this.Joint = joint;
            this.Parent = parent;

            CalculationLocalTransformation(this.Joint, this.Parent, out this.LocalTransformation);
        }

        public override bool Equals(object? obj)
        {
            return obj is Jointing linked &&
                   this.Joint == linked.Joint &&
                   this.Parent == linked.Parent;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Joint, Parent);
        }

        public bool Validate()
        {
            if(this.Joint.Entity == this.Parent.Entity)
            {
                return false;
            }

            if(this.Joint.Jointed)
            {
                return false;
            }

            if(this.Parent.Jointed)
            {
                return false;
            }

            return true;
        }

        public static bool operator ==(Jointing left, Jointing right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Jointing left, Jointing right)
        {
            return !left.Equals(right);
        }

        public static void CalculationLocalTransformation(Jointable.Joint joint, Jointable.Joint parent, out Matrix transformation)
        {
            transformation = Matrix.Invert(joint.Configuration.Transformation);
            transformation *= Matrix.CreateRotationZ(MathHelper.Pi);
            transformation *= parent.LocalTransformation;
        }
    }
}
