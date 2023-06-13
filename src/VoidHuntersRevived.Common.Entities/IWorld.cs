using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities.Systems;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IWorld
    {
        IEntityService Entities { get; }
        IComponentService Components { get; }
        ISystem[] Systems { get; }

        void Step(Step step);
    }
}
