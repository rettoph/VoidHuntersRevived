﻿using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Identity;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Game.Pieces;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Game.Ships.Engines
{
    [AutoLoad]
    internal sealed class UserEngine : BasicEngine, IGetReadyEngine,
        IEventEngine<UserJoined>
    {
        private readonly NetScope _scope;
        private readonly ITreeService _trees;

        public UserEngine(ITreeService trees, NetScope scope)
        {
            _scope = scope;
            _trees = trees;
        }

        public string name { get; } = nameof(UserEngine);

        public void Process(VhId id, UserJoined data)
        {
            VhId shipId = _scope.Peer!.Users.UpdateOrCreate(data.UserId, data.Claims).GetUserShipId();

            _trees.Spawn(shipId, EntityTypes.UserShip, EntityTypes.Pieces.HullSquare);
            // _treeFactory.Create(id.Create(1), EntityTypes.Chain, PieceTypes.HullSquare);
        }
    }
}
