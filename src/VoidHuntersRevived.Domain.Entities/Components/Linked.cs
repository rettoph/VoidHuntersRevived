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
    public sealed class Linked : EntityJoint
    {
        public readonly EntityJoint Parent;
        public readonly Matrix Transformation;

        public Linked(
            Entity entity, 
            int joint, 
            EntityJoint parent) : base(entity, joint)
        {
            this.Parent = parent;

            this.Transformation = this.Joint.AsChildTransformation;
            this.Transformation *= this.Parent.Joint.AsParentTransformation;

            if (this.Parent.Entity.TryGet<Linked>(out var parentLink))
            {
                this.Transformation *= parentLink.Transformation;
            }
        }

        public bool Validate()
        {
            if(this.Entity == this.Parent.Entity)
            {
                return false;
            }

            if(!this.Entity.TryGet<Linkable>(out var linkable) || !linkable.Contains(this.Joint))
            {
                return false;
            }

            if (!this.Parent.Entity.TryGet<Linkable>(out linkable) || !linkable.Contains(this.Parent.Joint))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Linked linked)
            {
                var result = this.Entity == linked.Entity;
                result &= this.Joint == linked.Joint;
                result &= this.Parent == linked.Parent;

                return result;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Entity, Joint, Parent);
        }

        public static bool operator ==(Linked linked1, Linked linked2)
        {
            return linked1.Equals(linked2);
        }

        public static bool operator !=(Linked linked1, Linked linked2)
        {
            return !linked1.Equals(linked2);
        }
    }
}
