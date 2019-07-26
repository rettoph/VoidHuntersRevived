using Guppy;
using Guppy.Collections;
using Guppy.Configurations;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.CustomEventArgs;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities.ConnectionNodes;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// The tractor beam entity is a simple object
    /// used to pick up and release free floating ship
    /// parts. It is constantly bound to a ship, and 
    /// must always be a certain "offset" relative to
    /// the ship.
    /// </summary>
    public class TractorBeam : Entity
    {
        #region Private Fields
        /// <summary>
        /// The distance a target may be from the ship and still get selected.
        /// </summary>
        private Single _selectionReach;
        private Single _atachmentReach;
        private Guid _targetFocus;
        private EntityCollection _entities;
        #endregion

        #region Public Attributes
        public readonly Ship Ship;
        public Boolean Selecting { get { return this.Selected != null; } }
        public ShipPart Selected { get; private set; }
        public Vector2 Offset { get; private set; }
        public Vector2 Position { get { return this.Ship.Bridge.Position + this.Offset; } }
        public Single Rotation { get; private set; }
        #endregion

        #region Events
        public event EventHandler<ShipPart> OnSelected;
        public event EventHandler<ShipPart> OnReleased;
        public event EventHandler<Vector2> OnOffsetChanged;
        public event EventHandler<Single> OnRotationChanged;
        public event EventHandler<ShipPart> OnAttached;
        #endregion

        #region Constructors
        public TractorBeam(Ship ship, EntityCollection entities, EntityConfiguration configuration, IServiceProvider provider) : base(configuration, provider)
        {
            _selectionReach = 20;
            _atachmentReach = 1f;
            _entities = entities;
            this.Ship = ship;
        }
        #endregion

        #region Frame Methods
        protected override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if(this.Selecting)
            { // Attempt to ajust the tractor beam rotation to the current selection's rotation
                this.SetRotation(this.Selected.Rotation);
            }
            this.UpdateSelectedPosition();
        }
        #endregion

        #region Interaction Methods
        /// <summary>
        /// Attempt to select a given target.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Boolean TrySelect(ShipPart target)
        {
            // Ensure that the proper target is recieved
            target = this.GetSelectionTarget(target);

            if (!this.ValidateSelectionTarget(target))
                return false;

            // If the target is connected to anything, release it 
            if (target.MaleConnectionNode.Connected)
            {
                // save the ship-parts old rotation, so we can set it again when it is released...
                var oldRotation = target.Rotation;
                // Attempt to detatch the target from the ship.
                target.TryDetatchFrom();
                // Reset the rotation value now that the target is free floating...
                target.Rotation = oldRotation;
            }

            // Attempt to release the old target from the tractor beam (if there is any)
            this.TryRelease();

            // Focus the new target
            this.Selected = target;
            this.Selected.SleepingAllowed = false;
            _targetFocus = this.Selected.Focused.Add();

            this.OnSelected?.Invoke(this, this.Selected);
            return true;
        }

        /// <summary>
        /// Attempt to release the current help ship part,
        /// if there is one.
        /// 
        /// On a successful release the OnReleased event will
        /// be triggered.
        /// </summary>
        /// <returns></returns>
        public Boolean TryRelease()
        {
            if (this.Selected == null)
                return false;

            var oldTarget = this.Selected;

            this.Selected.Focused.Remove(_targetFocus);
            this.Selected.SleepingAllowed = true;
            this.Selected = null;

            this.OnReleased?.Invoke(this, oldTarget);
            return true;
        }

        /// <summary>
        /// Attempt to attach the currently selected
        /// ship part to a given female connection node.
        /// 
        /// This will automatically release the ship-part.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Boolean TryAttatch(FemaleConnectionNode target)
        {
            if (!this.ValidateAttachmentTarget(target))
                return false;

            // Create the attachment...
            this.Selected.TryAttatchTo(target);
            this.OnAttached?.Invoke(this, this.Selected);

            return true;
        }

        public Boolean TryPreviewAttach(FemaleConnectionNode target)
        {
            if (!this.ValidateAttachmentTarget(target))
                return false;

            // Rather than creating the attachment, we just want to move the selection
            // so that a user can preview what it would look like when attached.
            var previewRotation = target.WorldRotation - this.Selected.MaleConnectionNode.LocalRotation;
            
            this.Selected.SetTransform(
                position: target.WorldPosition - Vector2.Transform(this.Selected.MaleConnectionNode.LocalPosition, Matrix.CreateRotationZ(previewRotation)),
                rotation: previewRotation);

            return true;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Check if a specified ship part can be selected by the
        /// current tractor beam.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Boolean ValidateSelectionTarget(ShipPart target)
        {
            // If there is no bridge bound to the current tractor beam...
            if (this.Ship.Bridge == null)
                return false;

            // If the target is null...
            if (target == null)
                return false;

            // If the ship part is focused (usually means selected by another tractor beam)
            if (target.Focused.Value)
                return false;

            // If the part is too far away from the current ship...
            if (Vector2.Distance(target.Position, this.Ship.Bridge.Position) > _selectionReach)
                return false;

            // If the target is attached to another ship...
            if (target.Root.IsBridge && target.Root.BridgeFor != this.Ship)
                return false;

            // If the target is the current ship bridge...
            if (this.Ship.Bridge == target)
                return false;

            return true;
        }

        /// <summary>
        /// If the input target is part of a chain, return the root most item
        /// That is, unless the target is part of a ship then return the target
        /// directly. If the target is not valid at all then return null.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public ShipPart GetSelectionTarget(ShipPart target)
        {
            if (!this.ValidateSelectionTarget(target))
                return default(ShipPart);
            if (target.Root.IsBridge)
                return target;

            return target.Root;
        }

        /// <summary>
        /// Validate whether or not the current selected
        /// ship part (if any) can attach to the given female
        /// connection node...
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Boolean ValidateAttachmentTarget(FemaleConnectionNode target)
        {
            // If the target is null 
            if (target == null)
                return false;

            // If the tractor beam isnt currently selecting something...
            if (!this.Selecting)
                return false;

            // If the target is too far away from the tractor beam...
            if (Vector2.Distance(target.WorldPosition, this.Position) > _atachmentReach)
                return false;

            // If the given female is already connected to something else...
            if (target.Connected)
                return false;

            // If the given connection node doesnt belong to the same ship as the tractor beam...
            if (target.Parent.Root.BridgeFor != this.Ship)
                return false;


            return true;
        }
        #endregion

        #region Position Methods
        public void SetOffset(Vector2 offset)
        {
            if (offset != this.Offset)
            {
                this.Offset = offset;

                this.UpdateSelectedPosition();

                this.OnOffsetChanged?.Invoke(this, this.Offset);
            }
        }
        public void SetRotation(Single rotation)
        {
            if (this.Rotation != rotation)
            {
                this.Rotation = rotation;

                this.UpdateSelectedPosition();

                this.OnRotationChanged?.Invoke(this, this.Rotation);
            }
        }

        public void UpdateSelectedPosition()
        {
            if (this.Selecting)
            { // If something is selected, update its position
                // Ensure the tractor beam is rotation to the parts rotation...
                this.Selected.SetTransform(
                    this.Position,
                    this.Rotation);
            }
        }
        #endregion

        #region Network Methods
        public void WriteOffsetData(NetOutgoingMessage om)
        {
            om.Write(this.Offset);
        }

        public void ReadOffsetData(NetIncomingMessage im)
        {
            this.SetOffset(
                im.ReadVector2());
        }

        public void WriteRotationData(NetOutgoingMessage om)
        {
            om.Write(this.Rotation);
        }

        public void ReadRotationData(NetIncomingMessage im)
        {
            this.SetRotation(
                im.ReadSingle());
        }

        public void WriteSelectedData(NetOutgoingMessage om)
        {
            om.Write(this.Selected);
        }

        public Boolean ReadSelectedData(NetIncomingMessage im)
        {
            return this.TrySelect(
                im.ReadEntity<ShipPart>(_entities));
        }
        #endregion
    }
}
