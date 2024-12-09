using Svelto.DataStructures;
using Svelto.ECS;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common
{

    public class EntityTag(string name)
    {
        private readonly FasterList<ExclusiveGroupStruct> _groups = new();

        public readonly string Name = name;

        public FasterReadOnlyList<ExclusiveGroupStruct> Groups => new(_groups);

        private static readonly Dictionary<string, EntityTag> _tags = [];
        public static EntityTag GetByName(string name)
        {
            ref EntityTag? tag = ref CollectionsMarshal.GetValueRefOrAddDefault(_tags, name, out bool exists);
            if (exists == false)
            {
                tag = new EntityTag(name);
            }

            return tag!;
        }

        public static EntityTag GetByKey(Key<IEntityTemplate> key)
        {
            return EntityTag.GetByName(key.Name);
        }

        public override bool Equals(object? obj)
        {
            return obj is EntityTag tag &&
                   Name == tag.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
