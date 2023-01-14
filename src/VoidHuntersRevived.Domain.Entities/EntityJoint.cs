using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities
{
    public class EntityJoint
    {
        public readonly Entity Entity;
        public readonly Linkable.Joint Joint;

        public EntityJoint(Entity entity, int joint)
        {
            this.Entity = entity;
            this.Joint = entity.Get<Linkable>()[joint];
        }

        public override bool Equals(object? obj)
        {
            return obj is EntityJoint joint &&
                   EqualityComparer<Entity>.Default.Equals(Entity, joint.Entity) &&
                   EqualityComparer<Linkable.Joint>.Default.Equals(Joint, joint.Joint);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Entity, Joint);
        }

        public static bool operator ==(EntityJoint joint1, EntityJoint joint2)
        {
            return joint1.Equals(joint2);
        }

        public static bool operator !=(EntityJoint joint1, EntityJoint joint2)
        {
            return !joint1.Equals(joint2);
        }
    }
}
