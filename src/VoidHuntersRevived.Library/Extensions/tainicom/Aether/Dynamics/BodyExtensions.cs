using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevivied.Library.Extensions.tainicom.Aether.Dynamics
{
    public static class BodyExtensions
    {
        public static void SetTransformIgnoreContacts(this AetherBody body, Vector2 position, float angle)
        {
            body.SetTransformIgnoreContacts(ref position, angle);
        }
    }
}
