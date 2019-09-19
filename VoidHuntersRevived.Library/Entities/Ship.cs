using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Utilities;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
        private Guid _bridgeReservationId;
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

        public TractorBeam TractorBeam { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.Events.Register<ShipPart>("bridge:changed");
            this.Events.Register<Direction>("direction:changed");
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Create the ship's tractor beam
            this.TractorBeam = this.entities.Create<TractorBeam>("tractor-beam");
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

            if (this.ActiveDirections.HasFlag(Direction.Forward))
                this.Bridge.ApplyLinearImpulse(Vector2.UnitY * -0.1f);
            if (this.ActiveDirections.HasFlag(Direction.Backward))
                this.Bridge.ApplyLinearImpulse(Vector2.UnitY * 0.1f);

            if (this.ActiveDirections.HasFlag(Direction.Left))
                this.Bridge.ApplyLinearImpulse(Vector2.UnitX * -0.1f);
            if (this.ActiveDirections.HasFlag(Direction.Right))
                this.Bridge.ApplyLinearImpulse(Vector2.UnitX * 0.1f);
        }
        #endregion

        #region Set Methods
        public void SetBridge(ShipPart bridge)
        {
            if(this.Bridge != bridge)
            {
                if(this.Bridge != null)
                {// Unreserve the old bridge
                    this.Bridge.Reserved.Remove(_bridgeReservationId);
                    this.Bridge.BridgeFor = null;
                    this.Bridge.SetCollidesWith(CollisionCategories.PassiveCollidesWith);
                    this.Bridge.SetCollisionCategories(CollisionCategories.PassiveCollisionCategories);
                }
                
                // Save & reserve the new bridge
                this.Bridge = bridge;
                this.Bridge.BridgeFor = this;
                this.Bridge.SetCollidesWith(CollisionCategories.ActiveCollidesWith);
                this.Bridge.SetCollisionCategories(CollisionCategories.ActiveCollisionCategories);
                _bridgeReservationId = this.Bridge.Reserved.Add();

                this.Events.TryInvoke<ShipPart>(this, "bridge:changed", this.Bridge);
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
