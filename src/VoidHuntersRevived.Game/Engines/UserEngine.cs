using Guppy.Attributes;
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
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Common.Components;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Pieces;
using VoidHuntersRevived.Game.Pieces.Components;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class UserEngine : BasicEngine,
        IEventEngine<UserJoined>
    {
        private readonly NetScope _scope;

        public UserEngine(NetScope scope)
        {
            _scope = scope;
        }

        public string name { get; } = nameof(UserEngine);

        public void Process(VhId id, UserJoined data)
        {
            VhId shipId = _scope.Peer!.Users.UpdateOrCreate(data.UserId, data.Claims).GetUserShipId();
            VhId square1Id = id.Create(1);
            VhId square2Id = id.Create(2);

            // BEGIN SHIP
            this.Simulation.Publish(CreateEntity.CreateEvent(
                type: EntityTypes.UserShip,
                vhid: shipId,
                initializer: (ref EntityInitializer initializer) =>
                {
                    initializer.Init(new Tree(square1Id));
                }));

            this.Simulation.Publish(CreateEntity.CreateEvent(
                type: PieceTypes.HullSquare, 
                vhid: square1Id,
                initializer: (ref EntityInitializer initializer) =>
                {
                    initializer.Init(new Node(shipId));
                }));
            // END SHIP

            // BEGIN CHAIN
            VhId chainId = id.Create(3);
            this.Simulation.Publish(CreateEntity.CreateEvent(
                type: EntityTypes.Chain,
                vhid: chainId,
                initializer: (ref EntityInitializer initializer) =>
                {
                    initializer.Init(new Tree(square2Id));
                }));

            this.Simulation.Publish(CreateEntity.CreateEvent(
                type: PieceTypes.HullSquare,
                vhid: square2Id,
                initializer: (ref EntityInitializer initializer) =>
                {
                    initializer.Init(new Node(chainId));
                }));
            // END CHAIN
        }
    }
}
