﻿using Guppy.DependencyInjection;
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

namespace VoidHuntersRevived.Library.Entities
{
    public class Ship : NetworkEntity
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
        private Vector2 _target;
        private ShipController _controller;
        private IList<ConnectionNode> _openFemaleNodes;
        private Player _player;
        #endregion

        #region Public Attributes
        /// <summary>
        /// The current player managing the ship (if any)
        /// </summary>
        public Player Player
        {
            get => _player;
            set
            {
                if(value != _player)
                {
                    _player = value;
                    this.OnPlayerChanged?.Invoke(this, _player);
                }
            }
        }

        /// <summary>
        /// The Ship's current target. This is a position relative 
        /// to the ship's current bridge's position.
        /// </summary>
        public Vector2 Target
        {
            get => _target;
            set
            {
                if(_target != value)
                {
                    _target = value;
                    this.OnTargetChanged?.Invoke(this, _target);
                }
            }
        }

        /// <summary>
        /// The calculated world position of the ship's current
        /// target.
        /// </summary>
        public Vector2 WorldTarget { 
            get => this.Bridge == default(ShipPart) ? Vector2.Zero : this.Bridge.WorldCenter + this.Target;
            set => this.Target = value - this.Bridge.WorldCenter;
        }

        /// <summary>
        /// The ships current bridge ship part (if any).
        /// </summary>
        public ShipPart Bridge { get; private set; }

        /// <summary>
        /// The current active Direction flags.
        /// </summary>
        public Direction ActiveDirections { get; private set; }

        public TractorBeam TractorBeam { get; private set; }

        public override GameAuthorization Authorization => this.Player == default(Player) ? base.Authorization : this.Player.Authorization;

        public IEnumerable<ConnectionNode> OpenFemaleNodes => _openFemaleNodes;

        public Color Color => Color.Cyan;
        #endregion

        #region Events
        public delegate void OnDirectionChangedDelegate(Ship sender, Ship.Direction direction, Boolean value);

        public event GuppyDeltaEventHandler<Ship, ShipPart> OnBridgeChanged;
        public event OnDirectionChangedDelegate OnDirectionChanged;
        public event GuppyEventHandler<Ship, Vector2> OnTargetChanged;
        public event GuppyEventHandler<Ship, Player> OnPlayerChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _openFemaleNodes = new List<ConnectionNode>();

            provider.Service(out _controller);

            this.TractorBeam = provider.GetService<TractorBeam>((t, p, c) => t.Ship = this);
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
                        this.Bridge.OnChainCleaned -= this.HandleBridgeCleaned;
                        this.Bridge.Ship = null;
                    }

                    this.logger.LogTrace($"Ship({this.Id}) => Setting bridge to ShipPart<{bridge.GetType().Name}>({bridge.Id})");
                    this.Bridge = bridge;
                    if (this.Bridge != null)
                    { // Setup the new bridge...
                        _controller.TryAdd(this.Bridge);
                        this.Bridge.Ship = this;
                        this.Bridge.OnChainCleaned += this.HandleBridgeCleaned;
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
            // Instance false
            if (!bridge.IsRoot)
                return false;

            // Instant true
            if (bridge == null)
                return true;
            else if (bridge.IsRoot)
                return true;

            // Default false
            return false;
        }

        /// <summary>
        /// Set a specified directional flag.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        public Boolean TrySetDirection(Direction direction, Boolean value)
        {
            if (value && !this.ActiveDirections.HasFlag(direction))
            {
                this.ActiveDirections |= direction;
                this.OnDirectionChanged?.Invoke(this, direction, value);
                return true;
            }
            else if (!value && this.ActiveDirections.HasFlag(direction))
            {
                this.ActiveDirections &= ~direction;
                this.OnDirectionChanged?.Invoke(this, direction, value);
                return true;
            }

            return false;
        }

        private void LoadOpenFemaleNodes()
        {
            _openFemaleNodes.Clear();
            this.Bridge?.GetOpenFemaleConnectionNodes(ref _openFemaleNodes);
        }

        /// <summary>
        /// Get the clostest available ConnectionNode to a recieved 
        /// WorldPosition
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public ConnectionNode GetClosestOpenFemaleNode(Vector2 position, Single range = 0.75f)
        {
            ConnectionNode closest = default(ConnectionNode);
            Single distance = range;

            foreach(ConnectionNode node in _openFemaleNodes)
                if (distance != (distance = Math.Min(Vector2.Distance(node.WorldPosition, position), distance)))
                    closest = node;

            return closest;
        }
        #endregion

        #region Event Handlers
        private void HandleBridgeCleaned(ShipPart sender, ShipPart.DirtyChainType arg)
            => this.LoadOpenFemaleNodes();
        #endregion
    }
}
