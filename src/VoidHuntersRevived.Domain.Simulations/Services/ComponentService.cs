using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Abstractions;

namespace VoidHuntersRevived.Domain.Simulations.Services
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

       

        public bool TryGet<T1>(EntityId key, out Ref<T1> component1)
            where T1 : unmanaged
        {
            if(!_entities.TryGetEGIDGroup(ref key, out EGIDGroup egidGroup))
            {
                component1 = default;
                return false;
            }

            component1.Instance = ref _entitiesDB.QueryEntity<Component<T1>>(egidGroup.EGID).Instance;

            return true;
        }

        public bool TryGet<T1, T2>(EntityId key, out Ref<T1> component1, out Ref<T2> component2)
            where T1 : unmanaged
            where T2 : unmanaged
        {
            if (!_entities.TryGetEGIDGroup(ref key, out EGIDGroup egidGroup))
            {
                component1 = default;
                component2 = default;
                return false;
            }

            NB<Component<T1>> component1Structs = _entitiesDB.QueryEntitiesAndIndex<Component<T1>>(egidGroup.EGID, out uint index);
            var (component2s, _) = _entitiesDB.QueryEntities<Component<T2>>(egidGroup.Group);

            component1.Instance = ref component1Structs[index].Instance;
            component2.Instance = ref component2s[index].Instance;

            return true;
        }

        public bool TryGet<T1, T2, T3>(EntityId key, out Ref<T1> component1, out Ref<T2> component2, out Ref<T3> component3)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
        {
            if (!_entities.TryGetEGIDGroup(ref key, out EGIDGroup egidGroup))
            {
                component1 = default;
                component2 = default;
                component3 = default;
                return false;
            }

            NB<Component<T1>> component1Structs = _entitiesDB.QueryEntitiesAndIndex<Component<T1>>(egidGroup.EGID, out uint index);
            var (component2s, _) = _entitiesDB.QueryEntities<Component<T2>>(egidGroup.Group);
            var (component3s, _) = _entitiesDB.QueryEntities<Component<T3>>(egidGroup.Group);

            component1.Instance = ref component1Structs[index].Instance;
            component2.Instance = ref component2s[index].Instance;
            component3.Instance = ref component3s[index].Instance;

            return true;
        }

        public bool TryGet<T1, T2, T3, T4>(EntityId key, out Ref<T1> component1, out Ref<T2> component2, out Ref<T3> component3, out Ref<T4> component4)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
        {
            if (!_entities.TryGetEGIDGroup(ref key, out EGIDGroup egidGroup))
            {
                component1 = default;
                component2 = default;
                component3 = default;
                component4 = default;
                return false;
            }

            NB<Component<T1>> component1Structs = _entitiesDB.QueryEntitiesAndIndex<Component<T1>>(egidGroup.EGID, out uint index);
            var (component2s, _) = _entitiesDB.QueryEntities<Component<T2>>(egidGroup.Group);
            var (component3s, _) = _entitiesDB.QueryEntities<Component<T3>>(egidGroup.Group);
            var (component4s, _) = _entitiesDB.QueryEntities<Component<T4>>(egidGroup.Group);

            component1.Instance = ref component1Structs[index].Instance;
            component2.Instance = ref component2s[index].Instance;
            component3.Instance = ref component3s[index].Instance;
            component4.Instance = ref component4s[index].Instance;

            return true;
        }

        public void Iterate<T1>(IterateDelegate<T1> iterator) where T1 : unmanaged
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = _entitiesDB.FindGroups<Component<EntityId>, Component<T1>>();
            foreach (var ((ids, component1s, count), _) in _entitiesDB.QueryEntities<Component<EntityId>, Component<T1>>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    iterator(in ids[i].Instance, ref component1s[i].Instance);
                }
            }
        }

        public void Iterate<T1, T2>(IterateDelegate<T1, T2> iterator)
            where T1 : unmanaged
            where T2 : unmanaged
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = _entitiesDB.FindGroups<Component<EntityId>, Component<T1>, Component<T2>>();
            foreach (var ((ids, component1s, component2s, count), _) in _entitiesDB.QueryEntities<Component<EntityId>, Component<T1>, Component<T2>>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    iterator(in ids[i].Instance, ref component1s[i].Instance, ref component2s[i].Instance);
                }
            }
        }

        public void Iterate<T1, T2, T3>(IterateDelegate<T1, T2, T3> iterator)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = _entitiesDB.FindGroups<Component<EntityId>, Component<T1>, Component<T2>, Component<T3>>();
            foreach (var ((ids, component1s, component2s, component3s, count), _) in _entitiesDB.QueryEntities<Component<EntityId>, Component<T1>, Component<T2>, Component<T3>>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    iterator(in ids[i].Instance, ref component1s[i].Instance, ref component2s[i].Instance, ref component3s[i].Instance);
                }
            }
        }

        public void Iterate<T1, T2, T3, T4>(IterateDelegate<T1, T2, T3, T4> iterator)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
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
