using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Tests.Common.Entities.Stubs;

namespace VoidHuntersRevived.Tests.Domain.Entities
{
    [Collection("EntityGroups")]
    public class EntityGroupList_Tests
    {

        [Fact]
        public void EntityGroupList_AutoAddExistingGroup()
        {
            EntityGroup.Clear();
            Assert.Empty(EntityGroup.GetAll());

            EntityGroupList.Clear();
            Assert.Empty(EntityGroupList.GetAll());
            _ = EntityGroup.Create("test", [typeof(TestEntityComponent)]);
            EntityGroupList groups = EntityGroupList.GetOrCreate([typeof(TestEntityComponent)]);
            Assert.Equal(1, groups.Values.count);
        }

        [Fact]
        public void EntityGroupList_EnsureMatchingComponentsReused()
        {
            EntityGroup.Clear();
            Assert.Empty(EntityGroup.GetAll());

            EntityGroupList.Clear();
            Assert.Empty(EntityGroupList.GetAll());
            _ = EntityGroup.Create("test", [typeof(TestEntityComponent), typeof(TestEntityComponent)]);

            EntityGroupList groups1 = EntityGroupList.GetOrCreate([typeof(TestEntityComponentTwo), typeof(TestEntityComponent)]);
            EntityGroupList groups2 = EntityGroupList.GetOrCreate([typeof(TestEntityComponent), typeof(TestEntityComponentTwo)]);

            bool refsEqual = object.ReferenceEquals(groups1, groups2);
            Assert.True(refsEqual);
        }
    }
}