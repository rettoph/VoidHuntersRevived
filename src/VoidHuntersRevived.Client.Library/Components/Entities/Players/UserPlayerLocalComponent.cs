using Guppy.CommandLine.Services;
using Guppy.DependencyInjection;
using Guppy.IO.Services;
using Guppy.Network.Components;
using Guppy.Network.Peers;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using VoidHuntersRevived.Library.Components.Entities.Ships;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Enums;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Entities.Aether;
using tainicom.Aether.Physics2D.Collision;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Structs;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Extensions.Lidgren;

namespace VoidHuntersRevived.Client.Library.Components.Entities.Players
{
    internal sealed class UserPlayerLocalComponent : RemoteHostComponent<UserPlayer>
    {
        #region Private Fields
        private Camera2D _camera;
        private ClientPeer _client;
        private CommandService _commands;
        private MouseService _mouse;
        private AetherWorld _world;
        private ShipPartService _shipPartService;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _camera);
            provider.Service(out _client);
            provider.Service(out _commands);
            provider.Service(out _mouse);
            provider.Service(out _world);
            provider.Service(out _shipPartService);

            if (_client.CurrentUser == this.Entity.User)
            {
                _commands.Get<Command>("ship set direction").Handler = CommandHandler.Create<Direction, Boolean, IConsole>(this.ShipSetDirectionCommandHandler);
                _commands.Get<Command>("ship tractorbeam").Handler = CommandHandler.Create<TractorBeamActionType, Single?, Single?, IConsole>(this.ShipTractorBeamCommandHandler);

                this.Entity.OnUpdate += this.Update;
            }
            else
            {
                this.Entity.ChunkLoader = false;
            }
        }

        protected override void Release()
        {
            base.Release();

            this.Entity.OnUpdate -= this.Update;

            _camera = default;
            _client = default;
            _commands = default;
            _mouse = default;
            _world = default;
            _shipPartService = default;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            if (this.Entity.Ship == default)
                return;

            _camera.MoveTo(this.Entity.Ship.Chain.Position);

            this.Entity.Ship.Components.Get<ShipTargetingComponent>().Target = _camera.Unproject(_mouse.Position);
        }
        #endregion

        #region Helper Methods
        private ShipPart GetShipPartTarget(Vector2 targetPosition)
        {
            // Sweep the world for any valid/selectable ShipParts.
            AABB aabb = new AABB(
                min: targetPosition - (Vector2.One * 2.5f),
                max: targetPosition + (Vector2.One * 2.5f));
            ShipPart target = default;
            Single targetDistance = Single.MaxValue, distance;

            _world.LocalInstance.QueryAABB(fixture =>
            {
                if (fixture.Tag is ShipPart shipPart)
                {
                    if (target == default)
                    {
                        target = shipPart;
                    }
                    else if ((distance = Vector2.Distance(targetPosition, shipPart.GetWorldPosition())) < targetDistance)
                    {
                        targetDistance = distance;
                        target = shipPart;
                    }
                }

                return true;
            }, ref aabb);

            return target;
        }
        #endregion

        #region Command Handlers
        private void ShipSetDirectionCommandHandler(Direction direction, Boolean value, IConsole arg3)
        {
            // TODO: Only broadcast when connected to remote peer.
            if(this.Entity.Ship?.Components.Get<ShipDirectionComponent>().TrySetDirection(direction, value) ?? false)
            { // Broadcast a message to the server alerting it of the directional update...
                this.CreateRequestDirectionChangedMessage(direction, value);
            }
        }

        private void ShipTractorBeamCommandHandler(TractorBeamActionType action, float? x, float? y, IConsole arg4)
        {
            if (this.Entity.Ship == default)
                return;

            // Calculate the target position of the recieved input coords.
            Vector2 targetPosition = this.Entity.Ship.Components.Get<ShipTractorBeamComponent>().GetValidTractorbeamPosition(
                worldPosition: new Vector2(
                    x: x ?? this.Entity.Ship.Components.Get<ShipTargetingComponent>().Target.X,
                    y: y ?? this.Entity.Ship.Components.Get<ShipTargetingComponent>().Target.Y
                )
            );

            // Generate a request tractor beam action request based on recieved action type.
            TractorBeamAction request = action switch
            {
                TractorBeamActionType.Select => new TractorBeamAction(
                    type: TractorBeamActionType.Select,
                    targetShipPart: this.GetShipPartTarget(targetPosition)),
                TractorBeamActionType.Deselect => new TractorBeamAction(
                    type: TractorBeamActionType.Deselect),
                TractorBeamActionType.Attach => new TractorBeamAction(
                    type: TractorBeamActionType.Attach),
                _ => throw new ArgumentOutOfRangeException(nameof(action))
            };

            // Attempt to invoke the request on the local tractor beam and save the response
            TractorBeamAction response = this.Entity.Ship.Components.Get<ShipTractorBeamComponent>().TryAction(request);

            // Nothing actually happened so... just stop?
            if (response.Type == TractorBeamActionType.None)
                return;

            // Broadcast the local response to the conencted peer
            this.CreateRequestTractorBeamActionMessage(response);
        }
        #endregion

        #region Network Methods
        private void CreateRequestDirectionChangedMessage(Direction direction, Boolean value)
        {
            this.Entity.Messages[VoidHuntersRevived.Library.Constants.Messages.UserPlayer.RequestDirectionChanged].Create(
                writer: om =>
                {
                    om.Write<Direction>(direction);
                    om.Write(value);
                },
                recipients: this.Entity.Pipe);
        }

        private void CreateRequestTractorBeamActionMessage(TractorBeamAction action)
        {
            this.Entity.Messages[VoidHuntersRevived.Library.Constants.Messages.UserPlayer.RequestTractorBeamAction].Create(
                writer: om => om.Write(action, _shipPartService, ShipPartSerializationFlags.None),
                recipients: this.Entity.Pipe);
        }
        #endregion
    }
}
