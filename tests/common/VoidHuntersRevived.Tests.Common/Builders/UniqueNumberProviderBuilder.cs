using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Providers;

namespace VoidHuntersRevived.Tests.Common.Builders
{
    public class UniqueNumberProviderBuilder : Builder<UniqueNumberProvider>
    {
        protected override UniqueNumberProvider Build()
        {
            return new UniqueNumberProvider();
        }
    }
}
