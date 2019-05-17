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
using VoidHuntersRevived.Library.Entities.Drivers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Client.Library.Entities.Drivers
{
    public class ClientPlayerDriver : PlayerDriver
    {
        private ClientPeer _client;
        private EntityCollection _entities;

        public ClientPlayerDriver(
            Player parent, 
            EntityConfiguration configuration, 
            Scene scene, 
            ILogger logger,
            ClientPeer client,
            EntityCollection entities) : base(parent, configuration, scene, logger)
        {
            _client = client;
            _entities = entities;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.parent.ActionHandlers.Add("update:bridge", this.HandleUpdateBridgeAction);
            this.parent.ActionHandlers.Add("update:direction", this.HandleUpdateDirectionAction);
            this.parent.ActionHandlers.Add("update:position", this.HandleUpdatePositionAction);
            this.parent.ActionHandlers.Add("update:velocity", this.HandleUpdateVelocityAction);
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

        private void HandleUpdatePositionAction(NetIncomingMessage obj)
        {
            this.parent.Bridge.Body.Position = obj.ReadVector2();
            this.parent.Bridge.Body.Rotation = obj.ReadSingle();
        }

        private void HandleUpdateVelocityAction(NetIncomingMessage obj)
        {
            this.parent.Bridge.Body.LinearVelocity = obj.ReadVector2();
            this.parent.Bridge.Body.AngularVelocity = obj.ReadSingle();
        }
        #endregion

        /// <summary>
        /// Update the player's directions locally, then send an action request to the server
        /// </summary>
        private void UpdateLocalDirection(Direction direction, Boolean value)
        {
            this.parent.UpdateDirection(direction, value);

            var action = this.parent.CreateActionMessage("update:direction");
            action.Write((Byte)direction);
            action.Write(value);
        }
    }
}
