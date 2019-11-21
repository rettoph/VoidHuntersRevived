using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Extensions.Farseer;

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
                dc.OnUpdateBody += this.HandleBodyUpdate;
            });

            this.SetEnabled(false);
            this.SetVisible(false);

            this.Events.Register<ShipPart>("selected");
            this.Events.Register<ShipPart>("released");
            // this.Events.Register<FemaleConnectionNode>("attached");
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
            // if (target == default(ShipPart))
            //     return true;
            // if (target.Controller is Chunk && target.IsRoot)
            //     return true;
            // if (!target.IsRoot && this.Ship.Components.Contains(target))
            //     return true;
            // 
            // return false;

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
            // if (this.ValidateTarget(component))
            //     return component;
            // else if (this.ValidateTarget(component?.Root))
            //     return component.Root;
            // else
            //     return default(ShipPart);

            return component;
        }

        /// <summary>
        /// Attempt to select a ShipPart instance
        /// </summary>
        /// <param name="target"></param>
        public void TrySelect(ShipPart target)
        {
            if(target != this.Selected)
            {
                this.TryRelease();

                if ((target = this.FindTarget(target)) != default(ShipPart))
                { // Only attempt anything if the recieved ship part is a valid target
                    // Detach the recieved target, if it is connected to anything
                    this.Selected = target;
                    // Add the target to the controller
                    _controller.Add(target);
                    // Trigger the selected event
                    this.Events.TryInvoke<ShipPart>(this, "selected", this.Selected);
                }
            }
        }

        /// <summary>
        /// Release the currently selected ShipPart &
        /// add it into the given controller. If no controller
        /// is provided it will default into the appropriate 
        /// chunk.
        /// </summary>
        /// <param name="controller"></param>
        public void TryRelease(Controller controller = default(Controller))
        {
            if(this.Selected != default(ShipPart))
            { // Only proceed if anything is selected
                // Add the selected object into the recieved controller or default to its current positional chunk
                (controller == default(Controller) ? _chunks.Get(this.Selected) : controller).Add(this.Selected);
                // Invoke the released event
                this.Events.TryInvoke<ShipPart>(this, "released", this.Selected);
                // Reset the contained selected item
                this.Selected = default(ShipPart);
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Automatically update a bodies position every frame
        /// </summary>
        /// <param name="component"></param>
        /// <param name="body"></param>
        private void HandleBodyUpdate(FarseerEntity component, Body body)
        {
            body.SetTransformIgnoreContacts(this.Position, body.Rotation);
        }
        #endregion
    }
}
