using Guppy.Tests.Common;
using Moq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Services
{
    public class EntityTemplateFragmentServiceMocker : Mocker<IEntityTemplateFragmentService>
    {
        private readonly List<EntityTemplateFragment> _fragments = [];

        public EntityTemplateFragmentServiceMocker()
        {
            this.Setup(x => x.GetAll(), () => _fragments.GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.ToArray()));
            this.Setup<EntityTemplateFragment[], Key<IEntityTemplate>>(x => x.GetByKey(It.IsAny<Key<IEntityTemplate>>()), key => _fragments.Where(x => x.Key == key).ToArray());
        }

        public void AddFragments(IEnumerable<EntityTemplateFragment> fragments)
        {
            _fragments.AddRange(fragments);
        }
    }
}
