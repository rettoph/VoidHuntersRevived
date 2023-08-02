using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Common.Ships.Components
{
    public struct Tactical : IEntityComponent
    {
        public FixVector2 Value;
        public FixVector2 Target;
        public int Uses;

        public void AddUse()
        {
            this.Uses++;
        }

        public void RemoveUse()
        {
            this.Uses = Math.Max(--this.Uses, 0);
        }
    }
}
