using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Physics
{
    public interface IFixture
    {
        EntityId EntityId { get; }

        IBody Body { get; }
    }
}
