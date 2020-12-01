using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Configurations;
using FarseerPhysics.Dynamics;
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

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart : BodyEntity
    {
        #region Private Fields
        private EntityList _entities;
        #endregion

        #region Public Properties
        public ShipPartConfiguration Configuration { get; set; }
        public Boolean IsRoot => !this.MaleConnectionNode.Attached;
        public ShipPart Root => this.Chain.Root;
        public ShipPart Parent => this.IsRoot ? null : this.MaleConnectionNode.Target.Parent;

        public Color Color => this.Chain.Ship?.Color ?? this.Root.Configuration.DefaultColor;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnChainChanged += this.HandleChainChanged;
            this.ValidateCleaning += this.HandleValidateCleaning;
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _entities);

            this.Transformations_PreInitialize(provider);

            this.BodyType = BodyType.Dynamic;

            this.Enabled = false;
            this.Visible = false;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            // Run _before_ Driven.Initialize to beat the Driver configuration.
            this.ConnectionNode_Initialize(provider);
            this.Chain_Initialize(provider);

            base.Initialize(provider);

            // Clean the chain once the ship part is initialized
            this.AngularDamping = 0.25f;
            this.LinearDamping = 0.25f;
        }

        protected override void Release()
        {
            base.Release();

            this.Transformations_Release();
            this.ConnectionNode_Release();
            this.Chain_Release();
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnChainChanged -= this.HandleChainChanged;
            this.ValidateCleaning -= this.HandleValidateCleaning;
        }
        #endregion

        #region Network Methods
        public void ReadMaleConnectionNode(NetIncomingMessage im)
        {
            if(im.ReadBoolean())
                this.MaleConnectionNode.TryAttach(
                    target: _entities.GetById<ShipPart>(im.ReadGuid()).FemaleConnectionNodes[im.ReadInt32()]);
        }

        public void WriteMaleConnectionNode(NetOutgoingMessage om)
        {
            if(om.WriteIf(this.MaleConnectionNode.Attached))
            {
                om.Write(this.MaleConnectionNode.Target.Parent.Id);
                om.Write(this.MaleConnectionNode.Target.Index);
            }
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
            if(old != default(Chain))
            { // Unset old chain actions as needed...
                // Remove the draw handler from the old chain
                old.OnDraw -= this.TryDraw;
            }

            if(value != default(Chain))
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
