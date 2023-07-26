using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Physics.Components;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Plug : IEntityComponent
    {
        public static readonly Plug Default = new Plug()
        {
            Location = new Location(FixVector2.UnitY / (Fix64)2, Fix64.Zero)
        };

        public required Location Location { get; init; }
    }
}
