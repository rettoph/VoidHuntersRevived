using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Enums
{
    [Flags]
    public enum TractorBeamActionType
    {
        None = 0,
        Select = 1,
        Deselect = 2,
        Attach = 4 | TractorBeamActionType.Deselect
    }
}
