using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Tests.Domain.Entities.Components;

namespace VoidHuntersRevived.Tests.Domain.Entities
{
    public class EntityGroup_Tests
    {
        [Fact]
        public void EntityGroup_AutoAddGroupToExistingGroupLists()
        {
            EntityGroupList groups = EntityGroupList.GetOrCreate([typeof(TestComponent)]);
            Assert.Equal(0, groups.Values.count);

            EntityGroup group = EntityGroup.Create("test", [typeof(TestComponent)]);
            Assert.Equal(1, groups.Values.count);
        }
    }
}
