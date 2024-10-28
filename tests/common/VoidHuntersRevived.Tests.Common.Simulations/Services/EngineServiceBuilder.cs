using Guppy.Core.Common;
using Guppy.Core.Messaging.Common.Services;
using Guppy.Tests.Common.Mocks;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Simulations.Services;
using VoidHuntersRevived.Tests.Common.Extensions;

namespace VoidHuntersRevived.Tests.Common.Simulations.Services
{
    public sealed class EngineServiceBuilder : ServiceBuilder<EngineService>
    {
        protected override EngineService Build(ServiceProviderMocker services)
        {
            return new EngineService(
                services.GetAll<IEngine>().ToFiltered(),
                services.Get<IBrokerService>(),
                new Lazy<IFiltered<IEngineProvider>>(() => new MockFiltered<IEngineProvider>()),
                services.Get<EnginesRoot>());
        }
    }
}
