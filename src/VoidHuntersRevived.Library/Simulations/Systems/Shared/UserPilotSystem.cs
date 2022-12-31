using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Providers;
using Guppy.Network.Identity.Services;
using Guppy.Network.Messages;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Components;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Services;
using static VoidHuntersRevived.Library.Helpers.EntityHelper;

namespace VoidHuntersRevived.Library.Simulations.Systems.Shared
{
    internal sealed class UserPilotSystem : EntitySystem, ISubscriber<UserPilot>
    {
        private readonly IUserProvider _userProvider;
        private World _world;
        private ComponentMapper<User> _users;
        private ComponentMapper<Piloting> _pilotings;
        private readonly ISimulationService _simulations;

        public UserPilotSystem(
            ISimulationService simulations,
            IUserProvider userProvider) : base(Aspect.All(typeof(User), typeof(Piloting)))
        {
            _userProvider = userProvider;
            _world = default!;
            _users = default!;
            _pilotings = default!;
            _simulations = simulations;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _world = world;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _users = mapperService.GetMapper<User>();
            _pilotings = mapperService.GetMapper<Piloting>();
        }

        public void Process(in UserPilot message)
        {
            var user = _userProvider.UpdateOrCreate(message.User.Id, message.User.Claims);

            switch (message.User.Action)
            {
                case UserAction.Actions.UserJoined:
                    foreach (ISimulation simulation in _simulations.Instances)
                    {
                        var pilot = Pilots.MakeUserPilot(
                            entity: simulation.GetEntity(message.PilotId),
                            user: user);

                        var ship = Rootables.MakeShip(
                            entity: simulation.GetEntity(new SimulatedId(SimulatedIdType.Ship, message.PilotId.Data)),
                            body: simulation.Aether.CreateRectangle(1, 1, 1, Vector2.Zero, 0, AetherBodyType.Dynamic));

                        _pilotings.Get(pilot).PilotableId = ship.Id;
                    }

                    _simulations.UserIdMap.Add(message.User.Id, message.PilotId);

                    break;
                default:
                    throw new Exception();
            }
        }
    }
}
