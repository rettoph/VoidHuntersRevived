using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    /// <summary>
    /// Can be thought of as the "male"
    /// connection
    /// </summary>
    public sealed class Linked
    {
        public readonly Linkable.Joint Joint;
        public readonly Linkable.Joint Parent;
        public readonly Matrix Transformation;

        public Linked(
            Linkable.Joint joint,
            Linkable.Joint parent)
        {
            this.Joint = joint;
            this.Parent = parent;

            this.Transformation = Matrix.Invert(this.Joint.Configuration.Transformation);
            this.Transformation *= Matrix.CreateRotationZ(MathHelper.Pi);
            this.Transformation *= this.Parent.Transformation;
        }

        public override bool Equals(object? obj)
        {
            return obj is Linked linked &&
                   this.Joint == linked.Joint &&
                   this.Parent == linked.Parent;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Joint, Parent);
        }

        public bool Validate()
        {
            if(this.Joint.Linkable == this.Parent.Linkable)
            {
                return false;
            }

            if(!this.Joint.Linkable.Joints.Contains(this.Joint))
            {
                return false;
            }

            if (this.Joint.Linkable.Joints.Contains(this.Parent))
            {
                return false;
            }

            return true;
        }

        public static bool operator ==(Linked left, Linked right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Linked left, Linked right)
        {
            return !left.Equals(right);
        }
    }
}
