using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Common
{
    internal static class AetherVector2Extensions
    {
        public static FixVector2 AsFixVector2(this AetherVector2 aetherVector2)
        {
            return Unsafe.As<AetherVector2, FixVector2>(ref aetherVector2);
        }

        public static AetherVector2 AsAetherVector2(this FixVector2 fixVector2)
        {
            return Unsafe.As<FixVector2, AetherVector2>(ref fixVector2);
        }
    }
}
