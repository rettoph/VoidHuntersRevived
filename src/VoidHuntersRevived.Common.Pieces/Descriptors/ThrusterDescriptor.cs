using Guppy.Resources;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public class ThrusterDescriptor : PieceDescriptor
    {
        public ThrusterDescriptor() : base(Resources.Colors.ThrusterPrimaryColor, Resources.Colors.ThrusterSecondaryColor, -1)
        {
        }
    }
}
