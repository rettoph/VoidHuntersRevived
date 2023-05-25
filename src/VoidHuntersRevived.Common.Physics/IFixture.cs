using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Physics
{
    public interface IFixture
    {
        ParallelKey EntityKey { get; }

        IBody Body { get; }
    }
}
