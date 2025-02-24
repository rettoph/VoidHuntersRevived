using Guppy.Tests.Common.Mocks;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Mockers
{
    public class EntitySerializationServiceMocker : MockBuilder<EntitySerializationService>
    {
        protected override EntitySerializationService Build()
        {
            throw new NotImplementedException();
        }
    }
}
