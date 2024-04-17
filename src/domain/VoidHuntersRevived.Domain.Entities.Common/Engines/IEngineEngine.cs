using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common.Engines
{
    public interface IEngineEngine : IEngine
    {
        void Initialize(IEngineService engines);
    }
}
