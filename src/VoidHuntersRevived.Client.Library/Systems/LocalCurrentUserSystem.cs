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
using VoidHuntersRevived.Library.Mappers;
using VoidHuntersRevived.Library.Systems;
using static VoidHuntersRevived.Library.Helpers.EntityHelper;

namespace VoidHuntersRevived.Client.Library.Systems
{
    internal sealed class LocalCurrentUserSystem : EntitySystem, ILockstepSimulationSystem,
        ISubscriber<Step>, ISortable, IDrawSystem
    {
        private readonly PilotIdMap _pilotIdMap;
        private readonly ClientPeer _client;
        private readonly Camera2D _camera;
        private ComponentMapper<Piloting> _pilots;
        private ComponentMapper<Body> _bodies;
        private GameTime _gameTime;

        public LocalCurrentUserSystem(ClientPeer client,
            PilotIdMap pilotIdMap,
            Camera2D camera) : base(Aspect.All(typeof(Piloting)))
        {
            _client = client;
            _pilotIdMap = pilotIdMap;
            _camera = camera;
            _pilots = default!;
            _bodies = default!;
            _gameTime = new GameTime();

            _camera.Zoom = 100;
        }

        public void Draw(GameTime gameTime)
        {
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
            var currentUserId = _client.Users.Current?.Id;
            if (currentUserId is null)
            {
                return;
            }

            if (!_pilotIdMap.TryGetEntityIdFromUserId(currentUserId.Value, out var pilotEntityId))
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

            _gameTime.ElapsedGameTime += message.Interval;
            _gameTime.TotalGameTime += message.Interval;
        }

    }
}
