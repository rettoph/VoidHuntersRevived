using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    public interface IEngineEngine : IEngine
    {
        void Initialize(IEngine[] engines);
    }
}
