using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IWorld
    {
        IEntitySerializationService Serialization { get; }
        IEntityService Entities { get; }
        IEngine[] Engines { get; }

        void Step(Step step);
    }
}
