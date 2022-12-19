﻿using Guppy.Common.Collections;
using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Mappers
{
    public abstract class PilotIdMap<TNetId>
        where TNetId : INetId
    {
        private INetId _current;
        private readonly Queue<INetId> _availableIds;
        private readonly Map<INetId, int> _entityIds;
        private readonly Map<INetId, int> _userIds;

        public PilotIdMap()
        {
            _availableIds = new Queue<INetId>();
            _entityIds = new Map<INetId, int>();
            _userIds = new Map<INetId, int>();
            _current = TNetId.Zero;
        }

        public INetId Add(int entityId)
        {
            var id = this.GetNextNetId();

            _entityIds.Add(id, entityId);

            return id;
        }

        public INetId Add(int entityId, int userId)
        {
            var id = this.Add(entityId);

            _userIds.Add(id, userId);

            return id;
        }

        public void Remove(INetId id)
        {
            _entityIds.Remove(id);
            _userIds.Remove(id);

            _availableIds.Enqueue(id);
        }

        public void RemoveByEntityId(int entityId)
        {
            var id = this.GetNetIdFromEntityId(entityId);

            this.Remove(id);
        }

        public void RemoveByUserId(int userId)
        {
            var id = this.GetNetIdFromUserId(userId);

            this.Remove(id);
        }

        public INetId GetNetIdFromUserId(int userId)
        {
            return _userIds[userId];
        }

        public INetId GetNetIdFromEntityId(int entityId)
        {
            return _entityIds[entityId];
        }

        public int GetEntityId(INetId id)
        {
            return _entityIds[id];
        }

        public int GetUserId(INetId id)
        {
            return _userIds[id];
        }

        public bool TryGetEntityIdFromUserId(int userId, [MaybeNullWhen(false)] out int entityId)
        {
            if(_userIds.TryGet(userId, out INetId? netId))
            {
                return _entityIds.TryGet(netId, out entityId);
            }

            entityId = default;
            return false;
        }

        private INetId GetNextNetId()
        {
            if(_availableIds.Count > 0)
            {
                return _availableIds.Dequeue();
            }

            _current = _current.Next();

            return _current;
        }
    }

    public sealed class PilotIdMap : PilotIdMap<NetId.Byte>
    {

    }
}
