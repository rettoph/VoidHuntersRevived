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
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Common.Components;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Pieces;
using VoidHuntersRevived.Game.Pieces.Components;
using VoidHuntersRevived.Game.Pieces.Properties;

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

            this.Simulation.Entities.Create(EntityNames.UserShip, shipId, (ref EntityInitializer initializer) =>
            {
                initializer.Get<Tree>().HeadId = this.Simulation.Entities.Create(PieceNames.HullSquare, id.Create(1)).VhId;
            });

            this.Simulation.Entities.Create(EntityNames.Chain, id.Create(2), (ref EntityInitializer initializer) =>
            {
                initializer.Get<Tree>().HeadId = this.Simulation.Entities.Create(PieceNames.HullSquare, id.Create(3)).VhId;
            });
        }
    }
}
