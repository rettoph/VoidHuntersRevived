using Guppy.Core.Common;
using Svelto.ECS;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public class EntityGroup
    {
        public readonly string Name;
        public readonly HashSet<Type> ComponentTypes;

        public readonly ExclusiveGroupStruct Value;

        internal EntityGroup(string name, IEnumerable<Type> componentTypes)
        {
            this.Name = name;
            this.ComponentTypes = new(componentTypes);
            this.Value = ExclusiveGroupStructHelper.GetOrCreateExclusiveStruct($"Group:{name}");

            foreach (EntityGroupList entityGroupList in EntityGroupList.GetAll())
            {
                if (this.Contains(entityGroupList.ComponentTypes) == true)
                {
                    entityGroupList._values.Add(this.Value);
                }
            }
        }

        private static readonly Dictionary<string, EntityGroup> _dictionary = [];
        public static EntityGroup Create(string name, IEnumerable<Type> components)
        {
            components.Any(ThrowIf.Type.IsNotAssignableFrom<IEntityComponent>);

            ref EntityGroup? group = ref CollectionsMarshal.GetValueRefOrAddDefault(_dictionary, name, out bool exists);
            if (exists == false)
            {
                group = new EntityGroup(name, components);
            }

            return group!;
        }

        public static IEnumerable<EntityGroup> GetAll()
        {
            return _dictionary.Values;
        }

        public bool Contains(IEnumerable<Type> componentTypes)
        {
            foreach (Type componentType in componentTypes)
            {
                if (this.ComponentTypes.Contains(componentType) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public bool Matches(IEnumerable<Type> componentTypes)
        {
            if (componentTypes.Count() != this.ComponentTypes.Count)
            {
                return false;
            }

            return this.Contains(componentTypes);
        }
    }
}
