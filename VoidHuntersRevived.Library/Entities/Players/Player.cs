using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Networking.Implementations;
using System.Linq;

namespace VoidHuntersRevived.Library.Entities.Players
{
    /// <summary>
    /// The player class holds everything that a single player can physically
    /// do when interacting with the world. The default player only contains
    /// possible actions, but does none of them itself. Look at extension player
    /// classes to see how this class is utilized.
    /// </summary>
    public class Player : NetworkEntity
    {
        #region Protected Fields
        protected MainGameScene GameScene;
        #endregion

        #region Public Attributes
        public ShipPart Bridge { get; private set; }
        public Dictionary<MovementType, Boolean> Movement { get; private set; }
        #endregion

        #region Constructors
        public Player(ShipPart bridge, EntityInfo info, IGame game) : base(info, game)
        {
            this.Bridge = bridge;
            this.Bridge.Body.SleepingAllowed = false;
        }

        public Player(long id, EntityInfo info, IGame game) : base(id, info, game)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Store the players main game scene
            this.GameScene = this.Scene as MainGameScene;

            // Create a default movement array
            Movement = new Dictionary<MovementType, Boolean>(6);
            Movement.Add(MovementType.GoForward, false);
            Movement.Add(MovementType.TurnRight, false);
            Movement.Add(MovementType.GoBackward, false);
            Movement.Add(MovementType.TurnLeft, false);
            Movement.Add(MovementType.StrafeRight, false);
            Movement.Add(MovementType.StrafeLeft, false);

            // Ensure the player is always enabled
            this.SetEnabled(true);
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Handle player movement
            foreach(KeyValuePair<MovementType, Boolean> kvp in Movement)
                if(kvp.Value)
                    switch (kvp.Key)
                    {
                        case MovementType.GoForward:
                            this.Bridge.Body.ApplyLinearImpulse(Vector2.Transform(new Vector2(4, 0), this.Bridge.RotationMatrix));
                            break;
                        case MovementType.TurnRight:
                            this.Bridge.Body.ApplyAngularImpulse(3f);
                            break;
                        case MovementType.GoBackward:
                            this.Bridge.Body.ApplyLinearImpulse(Vector2.Transform(new Vector2(-4, 0), this.Bridge.RotationMatrix));
                            break;
                        case MovementType.TurnLeft:
                            this.Bridge.Body.ApplyAngularImpulse(-4f);
                            break;
                        case MovementType.StrafeRight:
                            this.Bridge.Body.ApplyLinearImpulse(Vector2.Transform(new Vector2(0.1f, -4), this.Bridge.RotationMatrix));
                            break;
                        case MovementType.StrafeLeft:
                            this.Bridge.Body.ApplyLinearImpulse(Vector2.Transform(new Vector2(0.1f, 4), this.Bridge.RotationMatrix));
                            break;
                    }
        }
        #endregion

        #region INetworkEntity Implementation
        public override void Read(NetIncomingMessage im)
        {
            // Read the current Player's bridge
            this.Bridge = this.GameScene.NetworkEntities.GetById(im.ReadInt64()) as ShipPart;
            this.Bridge.Body.SleepingAllowed = false;


            for (Int32 i = 0; i < this.Movement.Count; i++) // Read the current movement settings
                this.Movement[(MovementType)im.ReadByte()] = im.ReadBoolean();
        }

        public override void Write(NetOutgoingMessage om)
        {
            om.Write(this.Id);

            // Write the current Player's bridge
            om.Write(this.Bridge.Id);


            // Write the current movement settings
            foreach (KeyValuePair<MovementType, Boolean> kvp in this.Movement)
            {
                om.Write((Byte)kvp.Key);
                om.Write(kvp.Value);
            }
        }
        #endregion
    }
}
