using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Structs
{
    public struct AppliedForce
    {
        public Vector2 Force { get; internal set; }
        public Vector2 Point { get; internal set; }
    }
}
