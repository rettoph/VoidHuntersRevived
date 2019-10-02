using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Entities.ShipParts.ConnectionNodes;
using GalacticFighters.Library.Utilities;
using Guppy.Loaders;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GalacticFighters.Library.Entities
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
        private List<ShipPart> _liveComponents;
        #endregion

        #region Public Attributes
        /// <summary>
        /// The current active Direction flags.
        /// </summary>
        public Direction ActiveDirections { get; private set; }

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
        /// A maintained enumerable of all internal live components
        /// </summary>
        public IEnumerable<ShipPart> LiveComponents { get => _liveComponents.AsReadOnly(); }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _openFemaleNodes = new List<FemaleConnectionNode>();
            _liveComponents = new List<ShipPart>();

            this.Events.Register<ShipPart>("bridge:changed");
            this.Events.Register<ShipPart>("bridge:chain:updated");
            this.Events.Register<Direction>("direction:changed");

            this.SetUpdateOrder(200);
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
            base.Update(gameTime);

            // Update all internal live components within the ship
            _liveComponents.ForEach(sp => sp.TryUpdate(gameTime));
        }
        #endregion

        #region Set Methods
        public void SetBridge(ShipPart bridge)
        {
            if(this.Bridge != bridge)
            {
                if(this.Bridge != null)
                {// Unreserve the old bridge
                    this.Bridge.BridgeFor = null;
                    this.Bridge.SetCollidesWith(CollisionCategories.PassiveCollidesWith);
                    this.Bridge.SetCollisionCategories(CollisionCategories.PassiveCollisionCategories);

                    // Remove bound events
                    this.Bridge.Events.TryRemove<ConnectionNode>("chain:updated", this.HandleChildNodeChanged);
                }
                
                // Save & reserve the new bridge
                this.Bridge = bridge;
                this.Bridge.BridgeFor = this;
                this.Bridge.SetCollidesWith(CollisionCategories.ActiveCollidesWith);
                this.Bridge.SetCollisionCategories(CollisionCategories.ActiveCollisionCategories);

                // Add events
                this.Bridge.Events.TryAdd<ConnectionNode>("chain:updated", this.HandleChildNodeChanged);

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

            // Clear all old components within the Ship's chain
            _liveComponents.Clear();
            // Reload all live components within the ship's chain
            this.Bridge?.GetAllChildren(ref _liveComponents, sp => sp == this.Bridge || sp.IsLive);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When a child node within the bridge is updated, we must remap
        /// open female nodes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleChildNodeChanged(object sender, ConnectionNode arg)
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
        }
        #endregion

        #region Network Methods
        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            this.WriteBridge(om);
        }

        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            this.ReadBridge(im);
        }

        /// <summary>
        /// Write the Ship's current bridge data
        /// </summary>
        /// <param name="om"></param>
        public void WriteBridge(NetOutgoingMessage om)
        {
            if (om.WriteExists(this.Bridge))
                om.Write(this.Bridge.Id);
        }

        /// <summary>
        /// Read & update the current bridge data
        /// </summary>
        /// <param name="im"></param>
        public void ReadBridge(NetIncomingMessage im)
        {
            if (im.ReadBoolean())
                this.SetBridge(this.entities.GetById<ShipPart>(im.ReadGuid()));
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
