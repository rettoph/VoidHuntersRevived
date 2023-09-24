using Guppy.Resources;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Serialization.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public class ThrusterDescriptor : PieceDescriptor
    {
        public ThrusterDescriptor() : base(Resources.Colors.ThrusterPrimaryColor, Resources.Colors.ThrusterSecondaryColor, -1)
        {
            this.ExtendWith(new[]
            {
                new ComponentManager<Thrustable, ThrustableComponentSerializer>()
            });
        }
    }
}
