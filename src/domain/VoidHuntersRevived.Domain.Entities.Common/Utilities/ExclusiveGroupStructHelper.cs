using Svelto.ECS;
using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Domain.Entities.Common.Utilities
{
    public static class ExclusiveGroupStructHelper
    {
        private static readonly Dictionary<string, ExclusiveGroup> _groups = [];

        public static ExclusiveGroupStruct GetOrCreateExclusiveStruct(string name)
        {
            ref ExclusiveGroup? group = ref CollectionsMarshal.GetValueRefOrAddDefault(_groups, name, out bool exists);

            if (exists)
            {
                return group!;
            }

            group = new ExclusiveGroup(name);
            return group;
        }
    }
}
