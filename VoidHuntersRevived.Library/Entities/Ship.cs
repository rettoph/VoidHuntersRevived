using System;
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

            _controller = this.entities.Create<CustomController>("entity:custom-controller", dc =>
            {
                dc.OnSetupBody += this.CustomBodySetup;
            });

            this.Events.Register<Direction>("direction:changed");
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the controller
            _controller.TryUpdate(gameTime);

            
            if(this.Bridge != default(ShipPart))
            { // Move the bridge
                if(this.ActiveDirections.HasFlag(Direction.Right))
                    this.Bridge.Body.ApplyForce(Vector2.UnitX * 10f, this.Bridge.Body.Position);
                if (this.ActiveDirections.HasFlag(Direction.Left))
                    this.Bridge.Body.ApplyForce(Vector2.UnitX * -10f, this.Bridge.Body.Position);
                if (this.ActiveDirections.HasFlag(Direction.Forward))
                    this.Bridge.Body.ApplyForce(Vector2.UnitY * -10f, this.Bridge.Body.Position);
                if (this.ActiveDirections.HasFlag(Direction.Backward))
                    this.Bridge.Body.ApplyForce(Vector2.UnitY * 10f, this.Bridge.Body.Position);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

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

        public void SetBridge(ShipPart target)
        {
            if(target != this.Bridge)
            { // Only proceed if the target is not already the current bridge...
                // Return all internal components back into their chunks
                while (_controller.Components.Any())
                    _chunks.AddToChunk(_controller.Components.First());

                // Add the new target into the internal controller
                _controller.Add(target);
                // Update the stored bridge value
                this.Bridge = target;
            }
        }
        #endregion

        #region Custom Controller Handlers
        private void CustomBodySetup(FarseerEntity component, Body body)
        {
            body.BodyType = BodyType.Dynamic;
        }
        #endregion

        #region Network Methods
        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            this.SetBridge(im.ReadEntity<ShipPart>(this.entities));
        }

        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            om.Write(_controller.Components.First());
        }
        #endregion
    }
}
