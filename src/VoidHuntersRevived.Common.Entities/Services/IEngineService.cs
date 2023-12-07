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
        void Initialize();

        IEnumerable<T> OfType<T>();

        T Get<T>();

        IEnumerable<IEngine> All();

        void Step(Step step);
    }
}
