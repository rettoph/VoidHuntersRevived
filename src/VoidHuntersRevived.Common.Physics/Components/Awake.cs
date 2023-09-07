using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Physics.Components
{
    public struct Awake : IEntityComponent
    {
        public readonly bool SleepingAllowed;
        public bool Value;

        public Awake(bool sleepingAllowed = true) : this()
        {
            this.SleepingAllowed = sleepingAllowed;
            this.Value = true;
        }

        public static implicit operator bool(Awake obj) => obj.Value;
    }
}
