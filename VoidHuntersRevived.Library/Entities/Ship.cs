using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.ConnectionNodes;
using VoidHuntersRevived.Library.Utilities;
using Guppy;
using Guppy.Loaders;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using VoidHuntersRevived.Library.Utilities.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// A ship represents a specific chain of pieces
    /// that can be controlled.
    /// </summary>
    public class Ship : NetworkEntity
    {
        #region Enums
        [Flags]
        public enum Direction
        {
            Forward = 1,
            Right = 2,
            Backward = 4,
            Left = 8,
            TurnLeft = 16,
            TurnRight = 32
        }
        #endregion

        #region Private Fields
        private List<FemaleConnectionNode> _openFemaleNodes;
        #endregion

        #region Internal Attributes
        internal ShipPartController controller { get; private set; }
        #endregion

        #region Public Attributes
        /// <summary>
        /// All synced ship parts within the ships current bridge chain
        /// </summary>
        public IReadOnlyCollection<ShipPart> Components { get => this.controller.Components; }

        /// <summary>
        /// The current active Direction flags.
        /// </summary>
        public Direction ActiveDirections { get; private set; }

        /// <summary>
        /// THe ship's current fire state
        /// </summary>
        public Boolean Firing { get; private set; }

        /// <summary>
        /// The ships current bridge.
        /// </summary>
        public ShipPart Bridge { get; private set; }

        /// <summary>
        /// The ship's internal tractor beam
        /// </summary>
        public TractorBeam TractorBeam { get; private set; }

        /// <summary>
        /// A maintained enumerable of open female connection nodes within
        /// the entire ship.
        /// </summary>
        public IEnumerable<FemaleConnectionNode> OpenFemaleNodes { get => _openFemaleNodes.AsReadOnly(); }

        /// <summary>
        /// The current target offset relative to the ship's bridge's
        /// position.
        /// </summary>
        public Vector2 TargetOffset { get; private set; }

        /// <summary>
        /// The world position of the target lock,
        /// if possible.
        /// </summary>
        public Vector2 Target { get => this.Bridge == default(ShipPart) ? Vector2.Zero : this.Bridge.WorldCenter + this.TargetOffset; }

        /// <summary>
        /// The current Ship's total size
        /// </summary>
        public Int32 Size { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _openFemaleNodes = new List<FemaleConnectionNode>();
            this.controller = provider.GetRequiredService<ShipPartController>();
            this.controller.CollidesWith = Categories.ActiveCollidesWith;
            this.controller.CollisionCategories = Categories.ActiveCollisionCategories;
            this.controller.IgnoreCCDWith = Categories.ActiveIgnoreCCDWith;
            this.controller.Color = new Color(9, 0, 255);

            this.Events.Register<ShipPart>("bridge:changed");
            this.Events.Register<ShipPart>("bridge:chain:updated");
            this.Events.Register<Direction>("direction:changed");
            this.Events.Register<Vector2>("target:offet:changed");
            this.Events.Register<Boolean>("firing:changed");

            this.SetUpdateOrder(100);
            this.SetDrawOrder(200);
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Create the ship's tractor beam
            this.TractorBeam = this.entities.Create<TractorBeam>("tractor-beam", tb =>
            {
                tb.Ship = this;
            });
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.Events.TryAdd<ShipPart>("bridge:chain:updated", this.HandleBridgeChainUpdated);

            this.RemapBridgeChain();
        }

        public override void Dispose()
        {
            base.Dispose();

            // Remove the ships old tractor beam
            this.TractorBeam.Dispose();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            // Update the ship itself
            base.Update(gameTime);
        }
        #endregion

        #region Set Methods
        public void SetBridge(ShipPart bridge)
        {
            if(this.Bridge != bridge)
            {
                // Ensure the tractor beam releases any targets on change
                this.TractorBeam?.TryRelease();

                if (this.Bridge != null)
                {
                    this.Bridge.Ship = null;

                    // Remove old events
                    this.Bridge.Events.TryRemove<ShipPart.ChainUpdate>("chain:updated", this.HandleBridgeShipPartChainUpdated);
                    this.Bridge.Events.TryRemove<Creatable>("disposing", this.HandleBridgeDisposing);
                }

                this.Bridge = bridge;
                if (this.Bridge != null)
                {
                    // Update the internal bridge value
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
        /// Set the ship's current firing state.
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

        public void SetTargetOffset(Vector2 offset)
        {
            if(offset != this.TargetOffset)
            {
                this.TargetOffset = offset;

                this.Events.TryInvoke<Vector2>(this, "target:offset:changed", this.TargetOffset);
            }
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
        public FemaleConnectionNode GetClosestOpenFemaleNode(Vector2 worldPosition, Single range = 0.75f)
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
            // Mark the internal chain dirty, preparing for claning next update
            this.controller.DirtyChain(this.Bridge);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When a child node within the bridge is updated, we must remap
        /// open female nodes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
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
        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            this.WriteBridge(om);
            this.WriteTargetOffset(om);
        }

        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            this.ReadBridge(im);
            this.ReadTargetOffset(im);
        }

        protected override void WritePostInitialize(NetOutgoingMessage om)
        {
            base.WritePostInitialize(om);

            this.WriteBridge(om);
            this.WriteTargetOffset(om);
        }

        protected override void ReadPostInitialize(NetIncomingMessage im)
        {
            base.ReadPostInitialize(im);

            this.ReadBridge(im);
            this.ReadTargetOffset(im);
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

        public void WriteTargetOffset(NetOutgoingMessage om)
        {
            om.Write(this.TargetOffset);
        }

        public void ReadTargetOffset(NetIncomingMessage im)
        {
            this.SetTargetOffset(im.ReadVector2());
        }
        #endregion
    }
}
