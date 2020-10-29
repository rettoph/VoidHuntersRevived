using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.DependencyInjection;
using Guppy.IO.Extensions.log4net;
using Guppy.Lists;
using Guppy.Events.Delegates;

namespace VoidHuntersRevived.Library.Entities
{
    public partial class Ship : NetworkEntity
    {
        #region Enums
        [Flags]
        public enum Direction
        {
            None = 0,
            Forward = 1,
            Right = 2,
            Backward = 4,
            Left = 8,
            TurnLeft = 16,
            TurnRight = 32
        }
        #endregion

        #region Private Fields
        private ShipController _controller;
        private IList<ConnectionNode> _openFemaleNodes;
        private Player _player;
        private EntityList _entities;
        #endregion

        #region Public Properties
        /// <summary>
        /// The current player managing the ship (if any)
        /// </summary>
        public Player Player
        {
            get => _player;
            set => this.OnPlayerChanged.InvokeIfChanged(value != _player, this, ref _player, value);
        }

        /// <summary>
        /// The ships current bridge ship part (if any).
        /// </summary>
        public ShipPart Bridge { get; private set; }

        public TractorBeam TractorBeam { get; private set; }

        public IEnumerable<ConnectionNode> OpenFemaleNodes => _openFemaleNodes;

        public Color Color => Color.Cyan;

        public String Title { get; set; } = "Unnamed Ship";
        #endregion

        #region Events
        public event OnChangedEventDelegate<Ship, ShipPart> OnBridgeChanged;
        public event OnChangedEventDelegate<Ship, Player> OnPlayerChanged;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.Network_Create(provider);
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _openFemaleNodes = new List<ConnectionNode>();

            provider.Service(out _controller);
            provider.Service(out _entities);

            this.TractorBeam = provider.GetService<TractorBeam>((t, p, c) => t.Ship = this);

            // Initialize partial classes.
            this.Events_PreIninitialize(provider);
            this.Directions_PreInitialize(provider);
            this.Targeting_PreInitialize(provider);
        }

        protected override void Release()
        {
            base.Release();

            // Dispose partial classes
            this.Directions_Dispose();
            this.Events_Dispose();
            this.Targeting_Dispose();
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.Network_Dispose();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _controller.TryUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _controller.TryDraw(gameTime);
        }
        #endregion

        #region Helper Methods
        public void SetBridge(ShipPart bridge)
        {
            if (this.Bridge != bridge)
            {
                if (this.ValidateBridge(bridge))
                { // Only proceed if the recieved bridge is valid...
                    var old = this.Bridge;
                    if (this.Bridge != null)
                    { // Remove old bridge...
                        this.Bridge.Chain.OnShipPartAdded -= this.HandleBridgeCleaned;
                        this.Bridge.Chain.OnShipPartRemoved -= this.HandleBridgeCleaned;
                        this.Bridge.Chain.Ship = null;
                    }

                    this.log.Verbose($"Ship({this.Id}) => Setting bridge to ShipPart<{bridge.GetType().Name}>({bridge.Id})");
                    this.Bridge = bridge;
                    if (this.Bridge != null)
                    { // Setup the new bridge...
                        _controller.TryAdd(this.Bridge.Chain);
                        this.Bridge.Chain.Ship = this;
                        this.Bridge.Chain.OnShipPartAdded += this.HandleBridgeCleaned;
                        this.Bridge.Chain.OnShipPartRemoved += this.HandleBridgeCleaned;
                    }

                    // Invoke the change event...
                    this.OnBridgeChanged?.Invoke(this, old, this.Bridge);
                    this.LoadOpenFemaleNodes();
                }
                else
                    throw new Exception("Invalid bridge value recieved.");
            }
        }

        /// <summary>
        /// Validate that a recieved ShipPart
        /// is a valid bridge for this ship.
        /// </summary>
        /// <returns></returns>
        private bool ValidateBridge(ShipPart bridge)
        {
            // Instant false
            if (!bridge.IsRoot)
                return false;

            // Instant true
            if (bridge == default(ShipPart))
                return true;
            else if (bridge.IsRoot)
                return true;

            // Default false
            return false;
        }



        private void LoadOpenFemaleNodes()
        {
            lock (_openFemaleNodes)
            {
                _openFemaleNodes.Clear();
                this.Bridge?.GetOpenFemaleConnectionNodes(ref _openFemaleNodes);
            }
        }

        /// <summary>
        /// Get the clostest available ConnectionNode to a recieved 
        /// WorldPosition
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public ConnectionNode GetClosestOpenFemaleNode(Vector2 position, Single range = 0.75f)
        {
            lock (_openFemaleNodes)
            {
                ConnectionNode closest = default(ConnectionNode);
                Single distance = range;

                foreach (ConnectionNode node in _openFemaleNodes)
                    if (distance != (distance = Math.Min(Vector2.Distance(node.WorldPosition, position), distance)))
                        closest = node;

                return closest;
            }
        }
        #endregion

        #region Event Handlers
        private void HandleBridgeCleaned(Chain sender, ShipPart arg)
            => this.LoadOpenFemaleNodes();
        #endregion
    }
}
