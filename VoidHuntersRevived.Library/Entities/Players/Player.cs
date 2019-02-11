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
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Library.Entities.ConnectionNodes;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using System.Collections;

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
        public TractorBeam TractorBeam { get; private set; }
        public ShipPart Bridge { get; private set; }
        public Dictionary<MovementType, Boolean> Movement { get; private set; }

        public FemaleConnectionNode[] OpenFemaleConnectionNodes { get; private set; }

        private Dictionary<MovementType, ArrayList> _thrusters;
        private Thruster[] _allThrusters;

        public Color Color { get; set; }
        #endregion

        #region Constructors
        public Player(ShipPart bridge, EntityInfo info, IGame game) : base(info, game)
        {
            this.SetBridge(bridge);
        }

        public Player(long id, EntityInfo info, IGame game) : base(id, info, game)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.UpdateOrder = 100;

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

            // Create a new TractorBeam instance for the current player
            this.TractorBeam = this.Scene.Entities.Create<TractorBeam>("entity:tractor_beam", null, this);

            // Create a new empty thrusters containers
            _thrusters = new Dictionary<MovementType, ArrayList>();
            _thrusters.Add(MovementType.GoForward  , new ArrayList());
            _thrusters.Add(MovementType.TurnRight  , new ArrayList());
            _thrusters.Add(MovementType.GoBackward , new ArrayList());
            _thrusters.Add(MovementType.TurnLeft   , new ArrayList());
            _thrusters.Add(MovementType.StrafeRight, new ArrayList());
            _thrusters.Add(MovementType.StrafeLeft , new ArrayList());

            // Update internal chain data
            this.ChainUpdated();

            // Create a new random color
            var rand = new Random();
            this.Color = new Color((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());

            // Ensure the player is always enabled
            this.SetEnabled(true);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Signaled when the bridge chain structure
        /// updates in any way . This will update the stored
        /// vairables, such as available female nodes, thruster
        /// configuration, and more
        /// </summary>
        internal void ChainUpdated()
        {
            this.Game.Logger.LogDebug("Player Chain Updated...");

            // Get a list of all open female nodes within the player
            this.OpenFemaleConnectionNodes = this.Bridge?.OpenFemaleConnectionNodes().ToArray();

            // Get a list of all thrusters on the ship
            _allThrusters = this.Bridge?.Children()
                .Where(sp => sp is Thruster)
                .Select(sp => sp as Thruster)
                .ToArray() ?? new Thruster[0];

            // Clear old thruster configuration
            _thrusters[MovementType.GoForward].Clear();
            _thrusters[MovementType.TurnRight].Clear();
            _thrusters[MovementType.GoBackward].Clear();
            _thrusters[MovementType.TurnLeft].Clear();
            _thrusters[MovementType.StrafeRight].Clear();
            _thrusters[MovementType.StrafeLeft].Clear();

            // Parse each thruster and find its relative position to the ship
            foreach (Thruster thruster in _allThrusters)
                foreach (KeyValuePair<MovementType, ArrayList> kvp in _thrusters)
                    if (thruster.MatchesMovementType(kvp.Key))
                        kvp.Value.Add(thruster);
        }

        /// <summary>
        /// Update the current players bridge
        /// </summary>
        /// <param name="shipPart"></param>
        internal void SetBridge(ShipPart bridge)
        {
            this.Bridge = bridge;
            this.Bridge.Body.SleepingAllowed = false;
            this.Bridge.SetEnabled(true);
            this.Bridge.BridgeFor = this;
            this.Bridge.DrawOrder = 100;

            // Update internal chain data
            if(this.InitializationState >= Core.Enums.InitializationState.Initializing)
                this.ChainUpdated();
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Disable all thrusters by default
            foreach (Thruster thruster in _allThrusters)
                thruster.SetActive(false);

            // Handle player movement
            foreach (KeyValuePair<MovementType, Boolean> kvp in Movement)
                if(kvp.Value)
                    foreach (Thruster thruster in _thrusters[kvp.Key])
                        thruster.SetActive(true); // Re-enable selected thrusters
        }
        #endregion

        #region INetworkEntity Implementation
        public override void Read(NetIncomingMessage im)
        {
            for (Int32 i = 0; i < this.Movement.Count; i++) // Read the current movement settings
                this.Movement[(MovementType)im.ReadByte()] = im.ReadBoolean();
        }

        public override void Write(NetOutgoingMessage om)
        {
            om.Write(this.Id);

            // Write the current movement settings
            foreach (KeyValuePair<MovementType, Boolean> kvp in this.Movement)
            {
                om.Write((Byte)kvp.Key);
                om.Write(kvp.Value);
            }
        }

        public override void FullRead(NetIncomingMessage im)
        {
            base.FullRead(im);

            // Read the current Player's bridge
            this.SetBridge(this.GameScene.NetworkEntities.GetById(im.ReadInt64()) as ShipPart);

            this.Color = new Color(im.ReadByte(), im.ReadByte(), im.ReadByte());
        }

        public override void FullWrite(NetOutgoingMessage om)
        {
            base.FullWrite(om);

            // Write the current Player's bridge
            om.Write(this.Bridge.Id);

            om.Write(this.Color.R);
            om.Write(this.Color.G);
            om.Write(this.Color.B);
        }
        #endregion
    }
}
