using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Components
{
    public sealed class Lockstepable
    {
        public static readonly Lockstepable Instance = new Lockstepable();
        private Lockstepable()
        {

        }
    }
}
