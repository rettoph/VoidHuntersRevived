﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Lidgren.Network;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Library.Utilities;
using Guppy;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Represents the main ship object.
    /// 
    /// Each ship has a single bridge (ShipPart)
    /// that acts as the main control point.
    /// </summary>
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
        private CustomController _controller;
        private ChunkCollection _chunks;
        private List<ConnectionNode> _openFemaleNodes;
        #endregion

        #region Public Properties
        /// <summary>
        /// The ships current bridge.
        /// </summary>
        public ShipPart Bridge { get; private set; }
        /// <summary>
        /// The current active Direction flags.
        /// </summary>
        public Direction ActiveDirections { get; private set; }

        /// <summary>
        /// The Ship's current target. This is a position relative 
        /// to the ship's current bridge's position.
        /// </summary>
        public Vector2 Target { get; private set; }

        /// <summary>
        /// The calculated world position of the ship's current
        /// target.
        /// </summary>
        public Vector2 WorldTarget { get => this.Bridge.WorldCenter + this.Target; }

        /// <summary>
        /// The current ship's tractor beam.
        /// </summary>
        public TractorBeam TractorBeam { get; private set; }

        /// <summary>
        /// A maintained enumerable of open female connection nodes within
        /// the entire ship.
        /// </summary>
        public IEnumerable<ConnectionNode> OpenFemaleNodes { get => _openFemaleNodes.AsReadOnly(); }

        /// <summary>
        /// The current Ship's total size
        /// </summary>
        public Int32 Size { get; private set; }

        /// <summary>
        /// Indicates that the ship is currently firing where possible.
        /// </summary>
        public Boolean Firing { get; private set; }
        #endregion

        #region Contructor
        public Ship(ChunkCollection chunks)
        {
            _chunks = chunks;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _openFemaleNodes = new List<ConnectionNode>();
            _controller = this.entities.Create<CustomController>("entity:custom-controller", dc =>
            {
                dc.OnSetupBody += this.CustomBodySetup;
            });

            this.Events.Register<ShipPart>("bridge:changed");
            this.Events.Register<ShipPart>("bridge:chain:updated");
            this.Events.Register<Direction>("direction:changed");
            this.Events.Register<Boolean>("firing:changed");
            this.Events.Register<Vector2>("target:changed");

            this.SetUpdateOrder(100);
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.TractorBeam = this.entities.Create<TractorBeam>("entity:tractor-beam", tb =>
            {
                tb.Ship = this;
            });

            this.Events.TryAdd<ShipPart>("bridge:chain:updated", this.HandleBridgeChainUpdated);

            this.RemapBridgeChain();
        }

        public override void Dispose()
        {
            base.Dispose();

            this.TractorBeam.Dispose();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the tractor beam
            this.TractorBeam.TryUpdate(gameTime);
            // Update the controller
            _controller.TryUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Draw the tractor beam
            this.TractorBeam.TryDraw(gameTime);

            // Draw the controller
            _controller.TryDraw(gameTime);
        }
        #endregion

        #region Setters
        /// <summary>
        /// Set a specified directional flag.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        public void SetDirection(Direction direction, Boolean value)
        {
            if (value && !this.ActiveDirections.HasFlag(direction))
            {
                this.ActiveDirections |= direction;
                this.Events.TryInvoke<Direction>(this, "direction:changed", direction);
            }
            else if (!value && this.ActiveDirections.HasFlag(direction))
            {
                this.ActiveDirections &= ~direction;
                this.Events.TryInvoke<Direction>(this, "direction:changed", direction);
            }
        }

        /// <summary>
        /// Update the ship's current firing status.
        /// </summary>
        /// <param name="value"></param>
        public void SetFiring(Boolean value)
        {
            if(this.Firing != value)
            {
                this.Firing = value;
                this.Events.TryInvoke<Boolean>(this, "firing:changed", this.Firing);
            }
        }

        public void SetBridge(ShipPart target)
        {
            if(target != this.Bridge)
            { // Only proceed if the target is not already the current bridge...
                // Auto release the tractor beam if possible
                this.TractorBeam?.TryRelease();

                

                if (this.Bridge != default(ShipPart))
                { // If the old bridge was not null...
                    this.Bridge.Ship = null;

                    // Remove old events
                    this.Bridge.Events.TryRemove<ShipPart.ChainUpdate>("chain:updated", this.HandleBridgeShipPartChainUpdated);
                    this.Bridge.Events.TryRemove<Creatable>("disposing", this.HandleBridgeDisposing);
                }

                // Update the stored bridge value
                this.Bridge = target;
                if (this.Bridge != default(ShipPart))
                { // If the new bridge is not null...
                    // Return all internal components back into their chunks
                    while (_controller.Components.Any())
                        _chunks.AddToChunk(_controller.Components.First());
                    // Add the new target into the internal controller
                    _controller.Add(target);
                    
                    // Update the new Bridge's Ship value
                    this.Bridge.Ship = this;

                    // Add new events
                    this.Bridge.Events.TryAdd<ShipPart.ChainUpdate>("chain:updated", this.HandleBridgeShipPartChainUpdated);
                    this.Bridge.Events.TryAdd<Creatable>("disposing", this.HandleBridgeDisposing);
                }

                // Invoke required events
                this.Events.TryInvoke<ShipPart>(this, "bridge:changed", this.Bridge);
                this.Events.TryInvoke<ShipPart>(this, "bridge:chain:updated", this.Bridge);
            }
        }

        /// <summary>
        ///  Update the current target value
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Vector2 target)
        {
            if (target != this.Target)
            {
                this.Target = target;

                this.Events.TryInvoke<Vector2>(this, "target:changed", this.Target);
            }
        }
        #endregion

        #region Custom Controller Handlers
        private void CustomBodySetup(FarseerEntity component, Body body)
        {
            body.CollisionCategories = Categories.ActiveCollisionCategories;
            body.CollidesWith = Categories.ActiveCollidesWith;
            body.IgnoreCCDWith = Categories.ActiveIgnoreCCDWith;
            body.BodyType = BodyType.Dynamic;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Get the closest open female connection node to a specified
        /// world position.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="range">The furthest away the female node can be to be considered valid</param>
        /// <returns></returns>
        public ConnectionNode GetClosestOpenFemaleNode(Vector2 worldPosition, Single range = 0.75f)
        {
            return this.OpenFemaleNodes
                .Where(f => Vector2.Distance(f.WorldPosition, worldPosition) <= range)
                .OrderBy(f => Vector2.Distance(f.WorldPosition, worldPosition))
                .FirstOrDefault();
        }

        /// <summary>
        /// Remap all required internal bridge chain data.
        /// 
        /// Such as open female node, life components, and more.
        /// </summary>
        private void RemapBridgeChain()
        {
            // Clear the connection node
            _openFemaleNodes.Clear();
            // Get all open female connection nodes within the bridge
            this.Bridge?.GetOpenFemaleConnectionNodes(ref _openFemaleNodes);
        }
        #endregion

        #region Event Handlers
        private void HandleBridgeShipPartChainUpdated(object sender, ShipPart.ChainUpdate arg)
        {
            this.Events.TryInvoke<ShipPart>(this, "bridge:chain:updated", this.Bridge);
        }

        /// <summary>
        /// When the ShipPart's chain is updated, we must remap open female nodes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleBridgeChainUpdated(object sender, ShipPart arg)
        {
            this.RemapBridgeChain();

            // Cache the chains current size
            this.Size = this.Bridge == default(ShipPart) ? 0 : this.Bridge.GetSize();
        }

        private void HandleBridgeDisposing(object sender, Creatable arg)
        {
            this.SetBridge(null);
        }
        #endregion

        #region Network Methods
        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            this.ReadBridge(im);
            this.ReadDirection(im);
            this.ReadDirection(im);
            this.ReadDirection(im);
            this.ReadDirection(im);
            this.ReadDirection(im);
            this.ReadDirection(im);
        }

        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            this.WriteBridge(om);
            this.WriteDirection(om, Direction.Forward);
            this.WriteDirection(om, Direction.Right);
            this.WriteDirection(om, Direction.Backward);
            this.WriteDirection(om, Direction.Left);
            this.WriteDirection(om, Direction.TurnLeft);
            this.WriteDirection(om, Direction.TurnRight);
        }

        /// <summary>
        /// Write the Ship's current bridge data
        /// </summary>
        /// <param name="om"></param>
        public void WriteBridge(NetOutgoingMessage om)
        {
            om.Write(this.Bridge);
        }

        /// <summary>
        /// Read & update the current bridge data
        /// </summary>
        /// <param name="im"></param>
        public void ReadBridge(NetIncomingMessage im)
        {
            this.SetBridge(im.ReadEntity<ShipPart>(this.entities));
        }


        /// <summary>
        /// Write a ship's specific direction data
        /// </summary>
        /// <param name="om"></param>
        /// <param name="direction"></param>
        public void WriteDirection(NetOutgoingMessage om, Direction direction)
        {
            om.Write((Byte)direction);
            om.Write(this.ActiveDirections.HasFlag(direction));
        }

        /// <summary>
        /// Read a ships specific direction data
        /// </summary>
        /// <param name="im"></param>
        public void ReadDirection(NetIncomingMessage im)
        {
            this.SetDirection((Direction)im.ReadByte(), im.ReadBoolean());
        }
        #endregion
    }
}
