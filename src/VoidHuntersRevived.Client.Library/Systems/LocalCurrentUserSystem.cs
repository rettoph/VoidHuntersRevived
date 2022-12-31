using Guppy.Attributes;
using Guppy.Common;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Components;
using VoidHuntersRevived.Library.Maps;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Simulations;
using VoidHuntersRevived.Library.Simulations.Systems;
using static VoidHuntersRevived.Library.Helpers.EntityHelper;

namespace VoidHuntersRevived.Client.Library.Systems
{
    internal sealed class LocalCurrentUserSystem : EntitySystem, ILockstepSimulationSystem,
        ISubscriber<Step>, ISortable, IDrawSystem
    {
        private readonly ISimulationService _simulations;
        private readonly ClientPeer _client;
        private readonly Camera2D _camera;
        private ComponentMapper<Piloting> _pilots;
        private ComponentMapper<Body> _bodies;

        public LocalCurrentUserSystem(ClientPeer client,
            ISimulationService simulations,
            Camera2D camera) : base(Aspect.All(typeof(Piloting)))
        {
            _client = client;
            _simulations = simulations;
            _camera = camera;
            _pilots = default!;
            _bodies = default!;

            _camera.Zoom = 100;
        }

        public void Draw(GameTime gameTime)
        {
            this.UpdateCameraTargets(gameTime);

            _camera.Update(gameTime);
        }

        public int GetOrder(Type enumerableType)
        {
            if(enumerableType == typeof(ISystem))
            {
                return int.MinValue;
            }

            if(enumerableType == typeof(ISubscriber))
            {
                return int.MaxValue;
            }

            return 0;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _pilots = mapperService.GetMapper<Piloting>();
            _bodies = mapperService.GetMapper<Body>();
        }

        public void Process(in Step message)
        {
            this.UpdateCameraTargets(message);
        }

        private void UpdateCameraTargets(GameTime gameTime)
        {
            var currentUserId = _client.Users.Current?.Id;
            if (currentUserId is null)
            {
                return;
            }

            if (!_simulations.UserIdMap.TryGet(currentUserId.Value, out var pilotId))
            {
                return;
            }

            if (!_simulations[SimulationType.Predictive].TryGetEntityId(pilotId, out var pilotEntityId))
            {
                return;
            }

            var piloting = _pilots.Get(pilotEntityId);
            if (piloting.PilotableId is null)
            {
                return;
            }

            var body = _bodies.Get(piloting.PilotableId.Value);
            if (body is null)
            {
                return;
            }

            _camera.TargetPosition = body.Position;
            _camera.TargetVelocity = body.LinearVelocity;
        }
    }
}
