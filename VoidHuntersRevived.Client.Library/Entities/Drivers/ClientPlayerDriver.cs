using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.Collections;
using Guppy.Configurations;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Peers;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities.Drivers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Client.Library.Entities.Drivers
{
    public class ClientPlayerDriver : PlayerDriver
    {
        private FarseerCamera2D _camera;
        private ClientPeer _client;
        private EntityCollection _entities;

        public ClientPlayerDriver(
            FarseerCamera2D camera,
            Player parent, 
            EntityConfiguration configuration, 
            Scene scene, 
            ILogger logger,
            ClientPeer client,
            EntityCollection entities) : base(parent, configuration, scene, logger)
        {
            _camera = camera;
            _client = client;
            _entities = entities;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.parent.ActionHandlers.Add("update:bridge", this.HandleUpdateBridgeAction);
            this.parent.ActionHandlers.Add("update:direction", this.HandleUpdateDirectionAction);
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if(this.parent.User == _client.CurrentUser)
            {
                var kState = Keyboard.GetState();

                if(kState.IsKeyDown(Keys.W) != this.parent.Directions[Direction.Forward])
                    this.UpdateLocalDirection(Direction.Forward, !this.parent.Directions[Direction.Forward]);
                if (kState.IsKeyDown(Keys.S) != this.parent.Directions[Direction.Backward])
                    this.UpdateLocalDirection(Direction.Backward, !this.parent.Directions[Direction.Backward]);
                if (kState.IsKeyDown(Keys.A) != this.parent.Directions[Direction.TurnLeft])
                    this.UpdateLocalDirection(Direction.TurnLeft, !this.parent.Directions[Direction.TurnLeft]);
                if (kState.IsKeyDown(Keys.D) != this.parent.Directions[Direction.TurnRight])
                    this.UpdateLocalDirection(Direction.TurnRight, !this.parent.Directions[Direction.TurnRight]);

                // Update the camera position
                if(this.parent.Bridge != null)
                    _camera.MoveTo(Vector2.Lerp(_camera.Position, this.parent.Bridge.Body.WorldCenter, 0.1f));
            }
        }

        public override void HandleUpdateBridge()
        {
            // throw new NotImplementedException();
        }

        public override void HandleUpdateDirection(Direction direction, bool value)
        {
            // throw new NotImplementedException();
        }

        #region Action Handlers
        private void HandleUpdateBridgeAction(NetIncomingMessage obj)
        {
            this.parent.UpdateBridge(_entities.GetById(obj.ReadGuid()) as ShipPart);
        }

        private void HandleUpdateDirectionAction(NetIncomingMessage obj)
        {
            this.parent.UpdateDirection(
                (Direction)obj.ReadByte(), 
                obj.ReadBoolean());
        }
        #endregion

        /// <summary>
        /// Update the player's directions locally,
        /// then send an action request to the server.
        /// 
        /// Generally this is used to predict what the server
        /// will tell the client to do, as the client claims
        /// ownership over the current player.
        /// </summary>
        private void UpdateLocalDirection(Direction direction, Boolean value)
        {
            if(value)
                this.parent.UpdateDirection(direction, value);

            var action = this.parent.CreateActionMessage("update:direction");
            action.Write((Byte)direction);
            action.Write(value);
        }
    }
}
