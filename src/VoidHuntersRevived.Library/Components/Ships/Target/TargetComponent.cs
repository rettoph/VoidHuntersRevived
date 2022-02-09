using Guppy.EntityComponent;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.Ships;

namespace VoidHuntersRevived.Library.Components.Ships
{
    public sealed class TargetComponent : Component<Ship>
    {
        public Vector2 Value { get; internal set; }

        internal void SetValue(Vector2 value)
        {
            this.Value = value;
        }
    }
}
