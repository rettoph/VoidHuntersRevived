using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Extensions.Farseer;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// The tractor beam represents an object that can interact with
    /// and pick up floating ShipPart objects.
    /// </summary>
    public class TractorBeam : Entity
    {
        #region Private Fields
        private CustomController _controller;
        private ChunkCollection _chunks;
        #endregion

        #region Public Properties
        /// <summary>
        /// The Ship that currently owns the tractor beam.
        /// </summary>
        public Ship Ship { get; internal set; }

        /// <summary>
        /// The world position of the tractor beam
        /// </summary>
        public Vector2 Position { get => this.Ship.WorldTarget; }

        /// <summary>
        /// The currently selected target, if any
        /// </summary>
        public ShipPart Selected { get; private set; }
        /// <summary>
        /// The rigid rotation th tractor beam is setting all selections
        /// to. This calus should auto update on select based on the 
        /// targets current rotation.
        /// </summary>
        public Single Rotation { get; set; }
        #endregion

        #region Events
        public event EventHandler<ShipPart> OnSelected;
        public event EventHandler<ShipPart> OnReleased;
        public event EventHandler<ConnectionNode> OnAttached;
        #endregion

        #region Constructor
        public TractorBeam(ChunkCollection chunks)
        {
            _chunks = chunks;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _controller = provider.GetRequiredService<EntityCollection>().Create<CustomController>("entity:custom-controller", dc =>
            {
                dc.OnSetupBody += this.HandleBodySetup;
                dc.OnUpdateBody += this.HandleBodyUpdate;
                dc.SetLocked(true);
            });

            this.SetEnabled(false);
            this.SetVisible(false);
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
        /// <summary>
        /// Check if a given ship part can be selected by the current tractorbeam.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Boolean ValidateTarget(ShipPart target)
        {
            // Instant yes...
            if (!target.Controller.Locked && target.IsRoot)
                return true;
            if (!target.IsRoot && target.Root.Ship?.Id == this.Ship.Id)
                return true;

            // Default to no
            return false;
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
        /// Attempt to select a ShipPart instance
        /// </summary>
        /// <param name="target"></param>
        public Boolean TrySelect(ShipPart target)
        {
            if(target != this.Selected)
            {
                this.TryRelease();
                
                if ((target = this.FindTarget(target)) != default(ShipPart))
                { // Only attempt anything if the recieved ship part is a valid target
                    target.MaleConnectionNode.Detach();
                    // Detach the recieved target, if it is connected to anything
                    this.Selected = target;
                    this.Rotation = this.Selected.Rotation;

                    // Add the target to the controller
                    _controller.Add(target);
                    // Trigger the selected event
                    this.OnSelected?.Invoke(this, this.Selected);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Release the currently selected ShipPart &
        /// add it into the given controller. If no controller
        /// is provided it will default into the appropriate 
        /// chunk.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns>The released ShipPart, if any.</returns>
        public ShipPart TryRelease()
        {
            if(this.Selected != default(ShipPart))
            { // Only proceed if anything is selected
                var oldSelected = this.Selected;
                // Reset the contained selected item
                this.Selected = default(ShipPart);
                // Add the selected object into the current positional chunk
                _chunks.AddToChunk(oldSelected);
                // Invoke the released event
                this.OnReleased?.Invoke(this, oldSelected);

                return oldSelected;
            }

            return default(ShipPart);
        }

        /// <summary>
        /// Attempt to attach the selected ShipPart
        /// to the given connection node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Boolean TryAttach(ConnectionNode node)
        {
            if (this.Selected != default(ShipPart))
            { // Only proceed if anything is selected
                node.Attach(this.Selected.MaleConnectionNode);
                // Reset the contained selected item
                this.Selected = default(ShipPart);
                // Invoke the released event
                this.OnAttached?.Invoke(this, node);

                return true;
            }

            return false;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Automatically setup the body configuration
        /// </summary>
        /// <param name="component"></param>
        /// <param name="body"></param>
        private void HandleBodySetup(FarseerEntity component, Body body)
        {
            body.CollisionCategories = Categories.PassiveCollisionCategories;
            body.CollidesWith = Categories.PassiveCollidesWith;
            body.IgnoreCCDWith = Categories.PassiveIgnoreCCDWith;
            body.BodyType = BodyType.Dynamic;
            body.ResetDynamics();
        }

        /// <summary>
        /// Automatically update a bodies position every frame
        /// </summary>
        /// <param name="component"></param>
        /// <param name="body"></param>
        private void HandleBodyUpdate(FarseerEntity component, Body body)
        {
            // Only update the position if its the root most piece
            if((component as ShipPart).IsRoot)
            {
                var node = this.Ship.GetClosestOpenFemaleNode(this.Position);

                if (node == default(ConnectionNode) || this.Selected == default(ShipPart))
                { // If there is no valid female node...
                    // Just move to where the target is...
                    body.SetTransformIgnoreContacts(
                        position: this.Position - Vector2.Transform(component.Configuration.GetData<ShipPartConfiguration>().Centeroid, Matrix.CreateRotationZ(body.Rotation)),
                        angle: this.Rotation);
                }
                else
                { // If there is a valid female node...
                    // Rather than creating the attachment, we just want to move the selection
                    // so that a user can preview what it would look like when attached.
                    this.Rotation = node.WorldRotation - this.Selected.MaleConnectionNode.LocalRotation;
                    // Update the preview position
                    body.SetTransformIgnoreContacts(
                        position: node.WorldPosition - Vector2.Transform(this.Selected.MaleConnectionNode.LocalPosition, Matrix.CreateRotationZ(this.Rotation)),
                        angle: this.Rotation);
                }
            }
        }
        #endregion
    }
}
