using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Engines
{
    public interface IEngineEngine : IEngine
    {
        void Initialize(IEngine[] engines);
    }
}
