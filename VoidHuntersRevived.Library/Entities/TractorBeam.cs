using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Entities.ShipParts.ConnectionNodes;
using Guppy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities
{
    /// <summary>
    /// Simple object used to pick up and interact with
    /// ShipPart objects.
    /// </summary>
    public sealed class TractorBeam : Entity
    {
        #region Private Attributes
        private Guid _selectionId;
        #endregion

        #region Public Attributes
        public ShipPart Selected { get; private set; }

        /// <summary>
        /// The beams current offset relative to its
        /// containing Ship
        /// </summary>
        public Vector2 Offset { get; private set; }

        /// <summary>
        /// The beams parent ship
        /// </summary>
        public Ship Ship { get; internal set; }

        public Vector2 Position { get => this.Ship.Bridge.WorldCenter + this.Offset; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.Events.Register<ShipPart>("selected");
            this.Events.Register<ShipPart>("released");
            this.Events.Register<ShipPart>("selected:position:changed");
            this.Events.Register<FemaleConnectionNode>("attached");
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.TryUpdateSelectedPosition();

            if (this.Selected != default(ShipPart))
                this.Selected.TryUpdate(gameTime);
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Check if a given ship part can be selected by the current tractorbeam.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Boolean ValidateTarget(ShipPart target)
        {
            if (target == default(ShipPart))
                return false;
            else if (target.IsBridge)
                return false;
            else if (target.Root.IsBridge && target.Root.BridgeFor?.TractorBeam != this)
                return false;
            else if (target.Reserved.Value)
                return false;
            else if (!target.IsRoot && !target.Root.IsBridge)
                return false;

            return true;
        }

        /// <summary>
        /// Return the selection target, if there is one, from
        /// a given chain component. This will automatically
        /// return the root piece on the chain, the current piece
        /// if you are detaching from the ship, or null if the piece
        /// in invalid.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public ShipPart FindTarget(ShipPart component)
        {
            if (this.ValidateTarget(component))
                return component;
            else if (this.ValidateTarget(component?.Root))
                return component.Root;
            else
                return default(ShipPart);
        }

        /// <summary>
        /// Attempt to select a recieved shippart targe.
        /// </summary>
        /// <param name="target"></param>
        public Boolean TrySelect(ShipPart target)
        {
            if (target != this.Selected)
            {
                this.TryRelease();

                if ((target = this.FindTarget(target)) != default(ShipPart))
                { // Only attempt anything if the recieved ship part is a valid target
                    // Detach the recieved target, if it is connected to anything
                    if (target.MaleConnectionNode.Attached)
                        target.MaleConnectionNode.Detach();

                    _selectionId = target.Reserved.Add();
                    this.Selected = target;

                    this.Events.TryInvoke<ShipPart>(this, "selected", this.Selected);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempt to release the current selected ship part, if there is any
        /// </summary>
        public Boolean TryRelease()
        {
            lock (this)
            {
                if (this.Selected != default(ShipPart))
                {
                    var oldSelected = this.Selected;

                    this.TryUpdateSelectedPosition();
                    this.Selected.Reserved.Remove(_selectionId);
                    this.Selected = null;

                    this.Events.TryInvoke<ShipPart>(this, "released", oldSelected);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Attempt to attach the current selected ship part to a given
        /// female connection node.
        /// 
        /// This will automatically release the node as well.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Boolean TryAttach(FemaleConnectionNode target)
        {
            lock (this)
            {
                var selected = this.Selected;

                if (target == default(FemaleConnectionNode))
                    return false;
                else if (target.Parent.Root != this.Ship.Bridge)
                    return false;
                else if (target.Attached)
                    return false;
                else if (this.Selected == default(ShipPart))
                    return false;

                selected.MaleConnectionNode.Attach(target);
                this.Events.TryInvoke<FemaleConnectionNode>(this, "attached", target);
                this.TryRelease();

                return true;
            }
        }

        public void SetOffset(Vector2 value)
        {
            this.Offset = value;
            this.TryUpdateSelectedPosition();
        }

        private void TryUpdateSelectedPosition()
        {
            lock (this)
            {
                if (this.Selected != default(ShipPart))
                {
                    this.Selected.SetPosition(this.Position - Vector2.Transform(this.Selected.LocalCenteroid, Matrix.CreateRotationZ(this.Selected.Rotation)), this.Selected.Rotation);

                    this.Events.TryInvoke<ShipPart>(this, "selected:position:changed", this.Selected);
                }
            }
        }
        #endregion
    }
}
