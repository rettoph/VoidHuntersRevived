using System.Runtime.InteropServices;
using System.Text;
using Guppy.Core.Common;
using Guppy.Core.Common.Extensions.System;
using Guppy.Core.Resources.Common.Extensions.System;
using Svelto.DataStructures;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public class EntityGroupList<T1>
        where T1 : unmanaged, IEntityComponent
    {
        private static readonly EntityGroupList _instance = EntityGroupList.GetOrCreate([typeof(T1)]);
        public static FasterReadOnlyList<ExclusiveGroupStruct> Values => _instance.Values;
    }

    public class EntityGroupList<T1, T2>
        where T1 : unmanaged, IEntityComponent
        where T2 : unmanaged, IEntityComponent
    {
        private static readonly EntityGroupList _instance = EntityGroupList.GetOrCreate([typeof(T1), typeof(T2)]);
        public static FasterReadOnlyList<ExclusiveGroupStruct> Values => _instance.Values;
    }

    public class EntityGroupList<T1, T2, T3>
        where T1 : unmanaged, IEntityComponent
        where T2 : unmanaged, IEntityComponent
        where T3 : unmanaged, IEntityComponent
    {
        private static readonly EntityGroupList _instance = EntityGroupList.GetOrCreate([typeof(T1), typeof(T2), typeof(T3)]);
        public static FasterReadOnlyList<ExclusiveGroupStruct> Values => _instance.Values;
    }

    public class EntityGroupList<T1, T2, T3, T4>
        where T1 : unmanaged, IEntityComponent
        where T2 : unmanaged, IEntityComponent
        where T3 : unmanaged, IEntityComponent
        where T4 : unmanaged, IEntityComponent
    {
        private static readonly EntityGroupList _instance = EntityGroupList.GetOrCreate([typeof(T1), typeof(T2), typeof(T3), typeof(T4)]);
        public static FasterReadOnlyList<ExclusiveGroupStruct> Values => _instance.Values;
    }

    public class EntityGroupList
    {
        internal readonly FasterList<ExclusiveGroupStruct> values;

        public readonly Guid Hash;
        public readonly string Name;
        public readonly HashSet<Type> ComponentTypes;

        public FasterReadOnlyList<ExclusiveGroupStruct> Values => new(this.values);

        internal EntityGroupList(Guid hash, string name, IEnumerable<Type> componentTypes)
        {
            this.values = new();

            this.Hash = hash;
            this.Name = name;
            this.ComponentTypes = new(componentTypes);

            foreach (EntityGroup group in EntityGroup.GetAll())
            {
                if (group.Contains(this.ComponentTypes))
                {
                    this.values.Add(group.Value);
                }
            }
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

        private static readonly Dictionary<Guid, EntityGroupList> _dictionary = [];
        public static EntityGroupList GetOrCreate(IEnumerable<Type> components)
        {
            _ = components.Any(ThrowIf.Type.IsNotAssignableFrom<IEntityComponent>);

            EntityGroupList.CalculateNameByComponentTypes(components, out Guid hash, out string name);
            ref EntityGroupList? group = ref CollectionsMarshal.GetValueRefOrAddDefault(_dictionary, hash, out bool exists);
            if (exists == false)
            {
                group = new EntityGroupList(hash, name, components);
            }

            return group!;
        }

        private static void CalculateNameByComponentTypes(IEnumerable<Type> componentTypes, out Guid hash, out string name)
        {
            StringBuilder stringBuilder = new();

            List<Type> orderedTypes = [.. componentTypes.OrderBy(x => x.AssemblyQualifiedName)];

            stringBuilder.AppendJoin('|', orderedTypes.Select(x => x.GetFormattedName()));
            name = stringBuilder.ToString();
            stringBuilder.Clear();

            stringBuilder.AppendJoin('|', orderedTypes.Select(x => x.AssemblyQualifiedName));
            hash = stringBuilder.ToString().xxHash128();
        }

        public static IEnumerable<EntityGroupList> GetAll() => _dictionary.Values;

        public static void Clear() => _dictionary.Clear();

        public override bool Equals(object? obj) => obj is EntityGroupList group &&
                   this.Name == group.Name;

        public override int GetHashCode() => HashCode.Combine(this.Name);
    }
}