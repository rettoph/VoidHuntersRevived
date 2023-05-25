using MonoGame.Extended.Entities;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public class ShipPart
    {
        public int? EntityId;
        public readonly Joint? Parent;
        public readonly ShipPart Head;
        public readonly FixLocation Location;

        public readonly Joint[] Joints;

        public readonly ShipPartComponent[] Components;

        public ShipPart(IEnumerable<FixLocation> joints, params ShipPartComponent[] components) : this(null, joints, components)
        {
        }
        private ShipPart(Joint? parent, IEnumerable<FixLocation> joints, params ShipPartComponent[] components)
        {
            this.Parent = parent;
            this.Head = this.Parent?.Owner.Head ?? this;
            this.Location = new FixLocation(FixVector2.Zero, Fix64.Zero);
            this.Components = components;

            this.Joints = joints.Select((x, idx) => new Joint(this, idx, x)).ToArray();
        }

        internal ShipPart Clone(Joint? parent)
        {
            ShipPart clone = new ShipPart(
                parent: parent, 
                joints: this.Joints.Select(x => x.Location), 
                components: this.Components.Select(x => x.Clone()).ToArray());

            foreach(Joint joint in this.Joints)
            {
                if(joint.Child is null)
                {
                    continue;
                }

                clone.Joints[joint.Index].Clone(joint.Child);
            }

            return clone;
        }

        public ShipPart Clone()
        {
            return this.Clone(null);
        }

        public void AttachTo(Entity entity)
        {
            entity.Attach(this);

            foreach(ShipPartComponent component in this.Components)
            {
                component.AttachTo(entity);
            }

            this.EntityId = entity.Id;
        }
    }
}
