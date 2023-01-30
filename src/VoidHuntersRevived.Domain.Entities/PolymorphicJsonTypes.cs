using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Entities
{
    internal static class PolymorphicJsonTypes
    {
        public const string ShipPartResource = "ShipPart";
        public const string DrawableConfiguration = "Draw";
        public const string RigidConfiguration = "Rigid";
        public const string JointableConfiguration = "Joint";
    }
}
