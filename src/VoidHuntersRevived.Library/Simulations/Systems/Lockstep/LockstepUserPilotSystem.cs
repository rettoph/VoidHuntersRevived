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
using VoidHuntersRevived.Library.Games;
using VoidHuntersRevived.Library.Helpers;
using VoidHuntersRevived.Library.Mappers;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Simulations.Systems.Lockstep
{
    internal sealed class LockstepUserPilotSystem : EntitySystem, ILockstepSimulationSystem, ISubscriber<UserPilot>
    {
        private readonly IUserProvider _userProvider;
        private World _world;
        private ComponentMapper<User> _users;
        private ComponentMapper<Piloting> _pilotings;
        private readonly SimulationEntityMapper _simulationEntityMapper;
        private readonly UserSimulationEntityMapper _userSimulationEntityMapper;
        private readonly LockstepSimulation _simulation;

        public LockstepUserPilotSystem(
            SimulationEntityMapper simulationEntityMapper,
            UserSimulationEntityMapper userSimulationEntityMapper,
            LockstepSimulation simulation,
            IUserProvider userProvider) : base(Aspect.All(typeof(User), typeof(Piloting)))
        {
            _userProvider = userProvider;
            _world = default!;
            _users = default!;
            _pilotings = default!;
            _simulationEntityMapper = simulationEntityMapper;
            _userSimulationEntityMapper = userSimulationEntityMapper;
            _simulation = simulation;
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

        protected override void OnEntityAdded(int entityId)
        {
            base.OnEntityAdded(entityId);

            if (subscription.IsInterested(entityId))
            {
                var user = _users.Get(entityId);

                var pilotId = _simulationEntityMapper.GetId(SimulationType.Lockstep, entityId);
                _userSimulationEntityMapper.Add(user.Id, pilotId);
            }
        }

        protected override void OnEntityRemoved(int entityId)
        {
            base.OnEntityRemoved(entityId);

            if (subscription.IsInterested(entityId))
            {
                var pilotId = _simulationEntityMapper.GetId(SimulationType.Lockstep, entityId);
                _userSimulationEntityMapper.RemoveById(pilotId);
            }
        }

        public void Process(in UserPilot message)
        {
            var user = _userProvider.UpdateOrCreate(message.User.Id, message.User.Claims);

            switch (message.User.Action)
            {
                case UserAction.Actions.UserJoined:
                    var pilot = EntityHelper.Pilots.CreateUserPilot(_world, user);
                    var ship = EntityHelper.Rootables.CreateShip(_world, _simulation.Aether.CreateRectangle(1, 1, 1, Vector2.Zero, 0, AetherBodyType.Dynamic));

                    _pilotings.Get(pilot).PilotableId = ship.Id;
                    _simulationEntityMapper.Set(message.PilotId, SimulationType.Lockstep, pilot.Id);
                    break;
                default:
                    throw new Exception();
            }
        }
    }
}
