using System.Collections;
using System.Collections.ObjectModel;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Common.Entities.ShipParts
{
    public partial class Joint
    {
        public readonly ShipPart Owner;
        public readonly int Index;
        public readonly FixLocation Location;

        public ShipPart? Child { get; private set; }

        public Joint(ShipPart owner, int index, FixLocation location)
        {
            this.Owner = owner;
            this.Index = index;
            this.Location = location;
        }

        /// <summary>
        /// Create a clone of a given ship part attached to the current joint.
        /// </summary>
        /// <param name="shipPart"></param>
        /// <exception cref="NotImplementedException"></exception>
        public ShipPart Clone(ShipPart head)
        {
            if(this.Child is not null)
            { // There is already a child defined.
                throw new NotImplementedException();
            }

            this.Child = head.Clone(this);

            return this.Child;
        }
    }
}
