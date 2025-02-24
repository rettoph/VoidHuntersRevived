using Guppy.Tests.Common;
using Moq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Mockers
{
    public class EntityTemplateFragmentServiceMocker : Mocker<IEntityTemplateFragmentService>
    {
        public List<EntityTemplateFragment> EntityTemplateFragments { get; }

        public EntityTemplateFragmentServiceMocker()
        {
            this.EntityTemplateFragments = [];

            this.SetupReturn(x => x.GetAll(), this.GetAllMock)
                .SetupReturn<EntityTemplateFragment[], Key<IEntityTemplate>>(x => x.GetByKey(It.IsAny<Key<IEntityTemplate>>()), this.GetByKeyMock)
                .SetupReturn(x => x.GetAllDistinctComponentTypes(), this.GetAllDistinctComponentTypesMock);
        }

        private IReadOnlyDictionary<Key<IEntityTemplate>, EntityTemplateFragment[]> GetAllMock()
        {
            return this.EntityTemplateFragments.GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.ToArray());
        }

        private EntityTemplateFragment[] GetByKeyMock(Key<IEntityTemplate> key)
        {
            return this.EntityTemplateFragments.Where(x => x.Key == key).ToArray();
        }

        public Type[] GetAllDistinctComponentTypesMock()
        {
            return this.EntityTemplateFragments.SelectMany(x => x.Components.Select(c => c.GetType())).Distinct().ToArray();
        }
    }
}
