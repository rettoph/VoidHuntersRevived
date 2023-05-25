using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common;

namespace tainicom.Aether.Physics2D.Dynamics
{
    public static class BodyExtensions
    {
        public static FixMatrix GetTransformation(this Body body)
        {
            return FixMatrix.CreateRotationZ(body.Rotation) * FixMatrix.CreateTranslation(body.Position.X, body.Position.Y, Fix64.Zero);
        }

        public static FixMatrix GetCenterTransformation(this Body body)
        {
            return FixMatrix.CreateTranslation(body.LocalCenter.X, body.LocalCenter.Y, Fix64.Zero) * body.GetTransformation();
        }

        public static FixMatrix GetLocalCenterTransformation(this Body body)
        {
            return FixMatrix.CreateTranslation(body.LocalCenter.X, body.LocalCenter.Y, Fix64.Zero);
        }
    }
}
