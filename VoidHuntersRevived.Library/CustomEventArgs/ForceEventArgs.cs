using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.CustomEventArgs
{
    public class ForceEventArgs : EventArgs
    {
        public readonly Vector2 Force;
        public readonly Vector2 Point;

        public ForceEventArgs(Vector2 force, Vector2 point)
        {
            this.Force = force;
            this.Point = point;
        }
    }
}
