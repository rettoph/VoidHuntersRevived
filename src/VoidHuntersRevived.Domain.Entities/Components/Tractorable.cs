using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    public sealed class Tractorable
    {
        public static readonly Tractorable Instance = new Tractorable();

        private Tractorable()
        {

        }
    }
}
