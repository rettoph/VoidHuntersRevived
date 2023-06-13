using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Abstractions;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal class ComponentService : IComponentService, IQueryingEntitiesEngine
    {
        private EntitiesDB _entitiesDB;
        private readonly EntityService _entities;

        public ComponentService(EntityService entities)
        {
            _entitiesDB = null!;
            _entities = entities;
        }

        public EntitiesDB entitiesDB { set => _entitiesDB = value; }

        public void Ready()
        {
            //
        }

       

        public bool TryGet<T1>(Guid key, out Ref<T1> component1)
            where T1 : unmanaged, IEntityComponent
        {
            if(!_entities.TryGetEGIDGroup(ref key, out EGIDGroup egidGroup))
            {
                component1 = default;
                return false;
            }

            component1.Instance = ref _entitiesDB.QueryEntity<T1>(egidGroup.EGID);

            return true;
        }

        public bool TryGet<T1, T2>(Guid key, out Ref<T1> component1, out Ref<T2> component2)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
        {
            if (!_entities.TryGetEGIDGroup(ref key, out EGIDGroup egidGroup))
            {
                component1 = default;
                component2 = default;
                return false;
            }

            NB<T1> component1Structs = _entitiesDB.QueryEntitiesAndIndex<T1>(egidGroup.EGID, out uint index);
            var (component2s, _) = _entitiesDB.QueryEntities<T2>(egidGroup.Group);

            component1.Instance = ref component1Structs[index];
            component2.Instance = ref component2s[index];

            return true;
        }

        public bool TryGet<T1, T2, T3>(Guid key, out Ref<T1> component1, out Ref<T2> component2, out Ref<T3> component3)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
        {
            if (!_entities.TryGetEGIDGroup(ref key, out EGIDGroup egidGroup))
            {
                component1 = default;
                component2 = default;
                component3 = default;
                return false;
            }

            NB<T1> component1Structs = _entitiesDB.QueryEntitiesAndIndex<T1>(egidGroup.EGID, out uint index);
            var (component2s, _) = _entitiesDB.QueryEntities<T2>(egidGroup.Group);
            var (component3s, _) = _entitiesDB.QueryEntities<T3>(egidGroup.Group);

            component1.Instance = ref component1Structs[index];
            component2.Instance = ref component2s[index];
            component3.Instance = ref component3s[index];

            return true;
        }

        public bool TryGet<T1, T2, T3, T4>(Guid key, out Ref<T1> component1, out Ref<T2> component2, out Ref<T3> component3, out Ref<T4> component4)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent
        {
            if (!_entities.TryGetEGIDGroup(ref key, out EGIDGroup egidGroup))
            {
                component1 = default;
                component2 = default;
                component3 = default;
                component4 = default;
                return false;
            }

            NB<T1> component1Structs = _entitiesDB.QueryEntitiesAndIndex<T1>(egidGroup.EGID, out uint index);
            var (component2s, _) = _entitiesDB.QueryEntities<T2>(egidGroup.Group);
            var (component3s, _) = _entitiesDB.QueryEntities<T3>(egidGroup.Group);
            var (component4s, _) = _entitiesDB.QueryEntities<T4>(egidGroup.Group);

            component1.Instance = ref component1Structs[index];
            component2.Instance = ref component2s[index];
            component3.Instance = ref component3s[index];
            component4.Instance = ref component4s[index];

            return true;
        }

        public void Iterate<T1>(IterateDelegate<T1> iterator, Step step) 
            where T1 : unmanaged, IEntityComponent
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = _entitiesDB.FindGroups<EntityId, T1>();
            foreach (var ((ids, component1s, count), _) in _entitiesDB.QueryEntities<EntityId, T1>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    iterator(step, in ids[i].Value, ref component1s[i]);
                }
            }
        }

        public void Iterate<T1, T2>(IterateDelegate<T1, T2> iterator, Step step)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = _entitiesDB.FindGroups<EntityId, T1, T2>();
            foreach (var ((ids, component1s, component2s, count), _) in _entitiesDB.QueryEntities<EntityId, T1, T2>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    iterator(step, in ids[i].Value, ref component1s[i], ref component2s[i]);
                }
            }
        }

        public void Iterate<T1, T2, T3>(IterateDelegate<T1, T2, T3> iterator, Step step)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = _entitiesDB.FindGroups<EntityId, T1, T2, T3>();
            foreach (var ((ids, component1s, component2s, component3s, count), _) in _entitiesDB.QueryEntities<EntityId, T1, T2, T3>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    iterator(step, in ids[i].Value, ref component1s[i], ref component2s[i], ref component3s[i]);
                }
            }
        }

        public void Iterate<T1, T2, T3, T4>(IterateDelegate<T1, T2, T3, T4> iterator, Step step)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent
        {
            throw new NotImplementedException();
            // LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = _entitiesDB.FindGroups<Component<T1>, Component<T2>, Component<T3>, Component<T4>>();
            // foreach (var ((ids, component1s, component2s, component3s, count), _) in _entitiesDB.QueryEntities<Component<T1>, Component<T2>, Component<T3>, Component<T4>>(groups))
            // {
            //     for (int i = 0; i < count; i++)
            //     {
            //         iterator(in ids[i].Instance, ref component1s[i].Instance, ref component2s[i].Instance, ref component3s[i].Instance);
            //     }
            // }
        }
    }
}
