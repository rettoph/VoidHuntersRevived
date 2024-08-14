using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common.Services;

namespace VoidHuntersRevived.Domain.Graphics.Providers
{
    [AutoLoad]
    public class PrimitiveInstanceEngineProvider : IEngineProvider
    {
        private readonly IPrimitiveService _primitiveService;

        public PrimitiveInstanceEngineProvider(IPrimitiveService primitiveService)
        {
            _primitiveService = primitiveService;
        }

        public IEnumerable<IEngine> GetEngines()
        {
            throw new NotImplementedException();
        }
    }
}
