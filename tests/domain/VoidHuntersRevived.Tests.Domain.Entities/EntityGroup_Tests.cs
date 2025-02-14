using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Tests.Common.Entities.Stubs;

namespace VoidHuntersRevived.Tests.Domain.Entities
{
    [Collection("EntityGroups")]
    public class EntityGroup_Tests
    {
        [Fact]
        public void EntityGroup_AutoAddGroupToExistingGroupLists()
        {
            EntityGroup.Clear();
            Assert.Empty(EntityGroup.GetAll());

            EntityGroupList.Clear();
            Assert.Empty(EntityGroupList.GetAll());


            EntityGroupList groups = EntityGroupList.GetOrCreate([typeof(TestEntityComponent)]);
            Assert.Equal(0, groups.Values.count);
            _ = EntityGroup.Create("test", [typeof(TestEntityComponent)]);
            Assert.Equal(1, groups.Values.count);
        }
    }
}