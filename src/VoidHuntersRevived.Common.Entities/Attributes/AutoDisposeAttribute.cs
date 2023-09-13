using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Common.Entities.Attributes
{
    public class AutoDisposeAttribute : Attribute
    {
        public readonly AutoDisposeScope Scope;

        public AutoDisposeAttribute(AutoDisposeScope scope)
        {
            Scope = scope;
        }
    }
}
