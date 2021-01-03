using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Contexts;
using Microsoft.Xna.Framework;
using Guppy.Interfaces;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Enums;
using Guppy.Extensions.DependencyInjection;
using Guppy.Events.Delegates;
using Lidgren.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Lists;
using VoidHuntersRevived.Library.Extensions.Lidgren.Network;
using tainicom.Aether.Physics2D.Dynamics;
using Guppy.Extensions.System;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart : BodyEntity
    {
        #region Private Fields
        private EntityList _entities;
        #endregion

        #region Public Properties
        /// <summary>
        /// The base <see cref="ShipPartContext"/> values.
        /// This determins vertices, hulls, colors, and more. This 
        /// value should be registered within a <see cref="ShipPartService"/>
        /// instance.
        /// </summary>
        public ShipPartContext Context { get; private set; }

        /// <summary>
        /// Determin whether or not the current <see cref="ShipPart"/>
        /// is the rootmost part of the <see cref="Chain"/>.
        /// </summary>
        public Boolean IsRoot => !this.MaleConnectionNode.Attached;

        /// <summary>
        /// Grab the rootmost <see cref="ShipPart"/> of the current <see cref="Chain"/>.
        /// This may return itself.
        /// </summary>
        public ShipPart Root => this.Chain.Root;

        /// <summary>
        /// Grabs the direct parent <see cref="ShipPart"/>, if any.
        /// </summary>
        public ShipPart Parent => this.IsRoot ? null : this.MaleConnectionNode.Target.Parent;

        /// <summary>
        /// The current <see cref="Microsoft.Xna.Framework.Color"/>, based on the 
        /// containing <see cref="Ship.Color"/> or <see cref="ShipPartContext.DefaultColor"/>
        /// as a fallback. The color value is then lerp'd to read based on the current
        /// <see cref="Health"/>.
        /// </summary>
        public Color Color => Color.Lerp(Color.Red, this.Context.InheritColor ? this.Chain.Ship?.Color ?? this.Root.Context.DefaultColor : this.Context.DefaultColor, this.Health / this.Context.MaxHealth);
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.Transformations_Create(provider);
            this.Network_Create(provider);

            this.OnChainChanged += this.HandleChainChanged;
            this.ValidateCleaning += this.HandleValidateCleaning;
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _entities);

            this.BodyType = BodyType.Dynamic;

            this.Enabled = false;
            this.Visible = false;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            // Run _before_ Driven.Initialize to beat the Driver configuration.
            this.ConnectionNode_Initialize(provider);
            this.Chain_Initialize(provider);
            this.Health_Initialize(provider);

            base.Initialize(provider);

            // Clean the chain once the ship part is initialized
            this.AngularDamping = 1f;
            this.LinearDamping = 0.25f;
        }

        protected override void PreRelease()
        {
            base.PreRelease();

            this.ConnectionNode_PreRelease();
        }

        protected override void Release()
        {
            base.Release();

            _entities = null;
            this.Chain_Release();
        }

        protected override void PostRelease()
        {
            base.PostRelease();
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnChainChanged -= this.HandleChainChanged;
            this.ValidateCleaning -= this.HandleValidateCleaning;

            this.Transformations_Dispose();
            this.Network_Dispose();
        }
        #endregion

        #region Methods
        public virtual void SetContext(ShipPartContext context)
        {
            this.Context = context;
        }
        #endregion

        #region Network Methods
        public void ReadMaleConnectionNode(NetIncomingMessage im)
        {
            if(im.ReadBoolean())
                this.MaleConnectionNode.TryAttach(
                    target: im.ReadConnectionNode(_entities));
        }

        public void WriteMaleConnectionNode(NetOutgoingMessage om)
        {
            if(om.WriteIf(this.MaleConnectionNode.Attached))
                om.Write(this.MaleConnectionNode.Target);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Reconfigure some internal actions when a ship part is added into a
        /// brand new chain...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="old"></param>
        /// <param name="value"></param>
        private void HandleChainChanged(ShipPart sender, Chain old, Chain value)
        {
            if(old != default)
            { // Unset old chain actions as needed...
                // Remove the draw handler from the old chain
                old.OnDraw -= this.TryDraw;
            }

            if(value != default)
            { // Set new chain actions as needed...
                // When the root chain is drawn, we should draw the current ship part as well
                value.OnDraw += this.TryDraw;
            }
        }

        private bool HandleValidateCleaning(NetworkEntity sender, GameTime args)
            => this.IsRoot;
        #endregion
    }
}
