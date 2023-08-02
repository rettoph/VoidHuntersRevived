using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Ships.Components
{
    public struct Tractorable : IEntityComponent
    {
        public bool IsTractored { get; set; }
    }
}
