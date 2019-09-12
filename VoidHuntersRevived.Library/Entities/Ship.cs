using GalacticFighters.Library.Entities.ShipParts;
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
        private Direction _direction;
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
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.Events.Register<ShipPart>("bridge:changed");
            this.Events.Register<Direction>("direction:changed");
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
                // Unreserve the old bridge
                this.Bridge?.Reserved.Remove(_bridgeReservationId);

                // Save & reserve the new bridge
                this.Bridge = bridge;
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
            if (value)
                this.ActiveDirections |= direction;
            else
                this.ActiveDirections &= ~direction;
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
        #endregion
    }
}
