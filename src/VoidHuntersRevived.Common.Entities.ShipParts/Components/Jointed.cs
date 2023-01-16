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
    public sealed class Jointed
    {
        public readonly Jointable.Joint Joint;
        public readonly Jointable.Joint Parent;
        public readonly Matrix LocalTransformation;

        public Jointed(
            Jointable.Joint joint,
            Jointable.Joint parent)
        {
            this.Joint = joint;
            this.Parent = parent;

            this.LocalTransformation = Matrix.Invert(this.Joint.Configuration.Transformation);
            this.LocalTransformation *= Matrix.CreateRotationZ(MathHelper.Pi);
            this.LocalTransformation *= this.Parent.LocalTransformation;
        }

        public override bool Equals(object? obj)
        {
            return obj is Jointed linked &&
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

        public static bool operator ==(Jointed left, Jointed right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Jointed left, Jointed right)
        {
            return !left.Equals(right);
        }
    }
}
