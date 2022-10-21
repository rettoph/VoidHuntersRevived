using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Enums
{
    [Flags]
    public enum TickFlags : byte
    {
        None = 0,
        UserPlayerJoined = 1,
    }
}
