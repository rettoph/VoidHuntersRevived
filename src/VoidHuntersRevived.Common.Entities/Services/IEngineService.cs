using Guppy.Common;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEngineService : IDisposable
    {
        EnginesRoot Root { get; }
        IEventPublishingService Events { get; }
        IEntityService Entities { get; }
        IEntitySerializationService Serialization { get; }
        IFilterService Filters { get; }

        IEngineService Load(params IState[] states);
        void Initialize();

        IEnumerable<T> OfType<T>();

        T Get<T>();

        IEnumerable<IEngine> All();

        void Step(Step step);
    }
}
