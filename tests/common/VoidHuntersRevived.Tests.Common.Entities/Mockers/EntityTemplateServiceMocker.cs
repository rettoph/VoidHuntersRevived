using Guppy.Tests.Common.Mocks;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Mockers
{
    public class EntityTemplateServiceMocker : MockBuilder<EntityTemplateService>
    {
        protected override EntityTemplateService Build()
        {
            throw new NotImplementedException();
        }
    }
}
