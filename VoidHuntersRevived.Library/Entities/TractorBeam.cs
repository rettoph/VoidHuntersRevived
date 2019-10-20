using FarseerPhysics.Dynamics;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.ConnectionNodes;
using Guppy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using VoidHuntersRevived.Library.Utilities.Controllers;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Simple object used to pick up and interact with
    /// ShipPart objects.
    /// </summary>
    public sealed class TractorBeam : Entity
    {
        #region Private Attributes
        private ShipPartController _controller;
        #endregion

        #region Public Attributes
        public ShipPart Selected { get; private set; }

        /// <summary>
        /// The beams parent ship
        /// </summary>
        public Ship Ship { get; internal set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _controller = provider.GetRequiredService<ShipPartController>();
            _controller.CollidesWith = Categories.PassiveCollidesWith;
            _controller.CollisionCategories = Categories.PassiveCollisionCategories;
            _controller.IgnoreCCDWith = Categories.PassiveIgnoreCCDWith;

            this.Events.Register<ShipPart>("selected");
            this.Events.Register<ShipPart>("released");
            this.Events.Register<ShipPart>("selected:position:changed");
            this.Events.Register<FemaleConnectionNode>("attached");
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.Ship.Events.TryAdd<Vector2>("target:offset:changed", this.HandleShipTargetOffsetChanged);

            this.SetEnabled(false);
            this.SetVisible(false);
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _controller.TryDraw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.TryUpdateSelectedPosition();

            _controller.TryUpdate(gameTime);
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
            else if (target.Root.IsControlled && !this.Ship.Components.Contains(target))
                return false;
            else if (!target.IsRoot && !target.Root.IsControlled)
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

                    this.Selected = target;
                    this.Selected.SetBodyEnabled(false);

                    _controller.SyncChain(this.Selected);

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
                    this.Selected.SetBodyEnabled(true);
                    this.Selected = null;

                    _controller.SyncChain(this.Selected);

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

        private void TryUpdateSelectedPosition()
        {
            if (this.Selected != default(ShipPart))
            { // Update the selected ship part, giving the user a placement preview
                var node = this.Ship.GetClosestOpenFemaleNode(this.Ship.Target);

                if (node == default(FemaleConnectionNode))
                {
                    // Calculate the absolute path to lerp towards
                    var selectedPositionTarget = this.Ship.Target - Vector2.Transform(this.Selected.LocalCenteroid, Matrix.CreateRotationZ(this.Selected.Rotation));

                    this.Selected.SetPosition(
                        Vector2.Lerp(this.Selected.Position, selectedPositionTarget, 0.25f), this.Selected.Rotation, true);
                }
                else
                { // Only proceed if there is a valid female node...
                  // Rather than creating the attachment, we just want to move the selection
                  // so that a user can preview what it would look like when attached.
                    var previewRotation = node.WorldRotation - this.Selected.MaleConnectionNode.LocalRotation;
                    // Update the preview position
                    this.Selected.SetPosition(
                        position: node.WorldPosition - Vector2.Transform(this.Selected.MaleConnectionNode.LocalPosition, Matrix.CreateRotationZ(previewRotation)),
                        rotation: previewRotation,
                        ignoreContacts: true);
                }

                this.Events.TryInvoke<ShipPart>(this, "selected:position:changed", this.Selected);
            }
        }
        #endregion

        #region Event Handlers
        private void HandleShipTargetOffsetChanged(object sender, Vector2 arg)
        {
            this.TryUpdateSelectedPosition();
        }
        #endregion
    }
}
