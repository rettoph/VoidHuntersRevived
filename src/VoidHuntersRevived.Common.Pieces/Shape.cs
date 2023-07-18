using Microsoft.Xna.Framework;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Pieces
{
    public struct Shape
    {
        public required EntityResource<Color> Color { get; init; }
        public required NativeDynamicArrayCast<Vector3> Vertices { get; init; }
    }
}
