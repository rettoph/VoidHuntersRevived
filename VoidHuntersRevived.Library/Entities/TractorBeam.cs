using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Controllers;
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
        #endregion

        #region Public Properties
        /// <summary>
        /// The world position of the tractor beam
        /// </summary>
        public Vector2 Position { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _controller = provider.GetRequiredService<EntityCollection>().Create<CustomController>("entity:custom-controller", dc =>
            {
                dc.OnUpdateBody += this.HandleBodyUpdate;
            });
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
