using Guppy.Core.Messaging.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Mocks;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Services
{
    public sealed class EngineServiceBuilder : BaseInstanceBuilder<EngineService>
    {
        public readonly MockFiltered<IEngine> Engines;
        public readonly Mocker<IBrokerService> BrokerService;
        public readonly MockFiltered<IEngineProvider> EngineProvider;
        public readonly Mocker<EntitiesSubmissionScheduler> EntitiesSubmissionScheduler;
        public readonly Mocker<EnginesRoot> EnginesRoot;

        public EngineServiceBuilder()
        {
            this.Engines = [];
            this.BrokerService = new Mocker<IBrokerService>();
            this.EngineProvider = [];
            this.EntitiesSubmissionScheduler = new Mocker<EntitiesSubmissionScheduler>();
            this.EnginesRoot = new Mocker<EnginesRoot>();
        }

        protected override EngineService build()
        {
            return new EngineService(
                this.Engines,
                this.BrokerService.GetInstance(),
                this.EngineProvider.ToLazy(),
                this.EnginesRoot.GetInstance(),
                this.EntitiesSubmissionScheduler.GetInstance());
        }
    }
}
