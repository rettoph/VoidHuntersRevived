using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common
{
    public readonly struct SimulatedId
    {
        public readonly SimulatedType Type;
        public readonly ushort Data;

        public SimulatedId(SimulatedType type, ushort data)
        {
            this.Type = type;
            this.Data = data;
        }
    }
}
