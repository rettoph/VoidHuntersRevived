using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Drivers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Server.Entities.Drivers
{
    class ServerUserPlayerDriver : Driver
    {
        #region Private Fields
        private MainGameScene _scene;
        #endregion

        #region Protected Fields
        protected UserPlayer UserPlayer;
        #endregion

        #region Constructors
        public ServerUserPlayerDriver(UserPlayer userPlayer, EntityInfo info, IGame game) : base(info, game)
        {
            this.UserPlayer = userPlayer;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _scene = this.Scene as MainGameScene;
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // UserPlayers should be synced every frame..
            if (!this.UserPlayer.Dirty)
                this.UserPlayer.Dirty = true;
        }
        #endregion

        #region Networking Methods (Driver Implementation)
        public override void Read(NetIncomingMessage im)
        {
            if (im.SenderConnection.RemoteUniqueIdentifier == this.UserPlayer.User.Id)
            { // Only read the data if the sender connection id matches the user id

                // Read the current movement settings
                for (Int32 i = 0; i < this.UserPlayer.Movement.Count; i++) 
                    this.UserPlayer.Movement[(MovementType)im.ReadByte()] = im.ReadBoolean();

                this.UserPlayer.TractorBeam.Body.Position = im.ReadVector2();

                var tractorBeamHeld = im.ReadBoolean();
                if (tractorBeamHeld && this.UserPlayer.TractorBeam.Connection == null)
                    this.UserPlayer.TractorBeam.Select(); // Attempt to select the hovered ShipPart
                else if (!tractorBeamHeld && this.UserPlayer.TractorBeam.Connection != null)
                    this.UserPlayer.TractorBeam.Connection.Disconnect(); // Attempt to disconnect the current tractor beam connection
            }
            else
            { // Someone tried spoofing another players ship...
                im.SenderConnection.Disconnect("Invalid message.");
            }
        }

        public override void Write(NetOutgoingMessage om)
        {
            om.Write(this.UserPlayer.TractorBeam.Body.Position);
        }
        #endregion
    }
}
