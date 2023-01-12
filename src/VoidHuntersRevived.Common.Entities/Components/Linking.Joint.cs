using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public partial class Linking
    {
        public class Joint
        {
            public Vector2 Position { get; set; }
            public float Rotation { get; set; }
        }
    }
}
