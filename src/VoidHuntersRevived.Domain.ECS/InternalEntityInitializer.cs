using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS;
using VoidHuntersRevived.Domain.ECS.Abstractions;

namespace VoidHuntersRevived.Domain.ECS
{
    internal sealed class InternalEntityInitializer : IEntityInitializer
    {
        private readonly EntitiesDB _entitiesDB;
        private readonly EGIDGroup _egidGroup;
        private uint? _index;

        public EntityId Key { get; }
        public EntityType Type { get; }

        public InternalEntityInitializer(EntitiesDB entities, EGIDGroup egidGroup, EntityId key, EntityType type)
        {
            _entitiesDB = entities;
            _egidGroup = egidGroup;
            this.Key = key;
            this.Type = type;
        }

        public ref T Get<T>()
            where T : unmanaged
        {
            if (_index is null)
            {
                NB<Component<T>> componentStructs = _entitiesDB.QueryEntitiesAndIndex<Component<T>>(_egidGroup.EGID, out uint index);
                _index = index;
                return ref componentStructs[index].Instance;
            }

            var (components, _) = _entitiesDB.QueryEntities<Component<T>>(_egidGroup.Group);
            return ref components[_index.Value].Instance;
        }

        public bool Has<T>() where T : unmanaged
        {
            return _entitiesDB.HasAny<Component<T>>(_egidGroup.Group);
        }

        public void Set<T>(in T component) where T : unmanaged
        {
            ref T componentRef = ref this.Get<T>();
            componentRef = component;
        }
    }
}
