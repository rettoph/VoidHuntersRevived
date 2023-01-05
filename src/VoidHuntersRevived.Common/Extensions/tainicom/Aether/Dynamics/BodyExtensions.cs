using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tainicom.Aether.Physics2D.Dynamics
{
    public static class BodyExtensions
    {
        public static void SetTransformIgnoreContacts(this Body body, Vector2 position, float angle)
        {
            body.SetTransformIgnoreContacts(ref position, angle);
        }
    }
}
