using Guppy.Common;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEngineService : IDisposable
    {
        EnginesRoot Root { get; }
        IEntityService Entities { get; }
        IEntitySerializationService Serialization { get; }

        IEngineService Initialize(params IState[] states);

        IEnumerable<T> OfType<T>();

        T Get<T>();

        IEnumerable<IEngine> All();

        void Step(Step step);
    }
}
