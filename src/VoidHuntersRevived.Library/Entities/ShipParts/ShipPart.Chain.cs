using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Events.Delegates;
using Guppy.Extensions.System.Collections;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// Partial ShipPart implementation specifically
    /// managing chain related functionality.
    /// </summary>
    public partial class ShipPart
    {
        #region Private Fields
        private Chain _chain;
        private ServiceProvider _provider;
        #endregion

        #region Public Properties
        public Chain Chain
        {
            get => _chain;
            internal set => this.OnChainChanged.InvokeIf(value != _chain, this, ref _chain, value);
        }
        #endregion

        #region Events
        public event OnChangedEventDelegate<ShipPart, Chain> OnChainChanged;
        #endregion

        #region Lifecycle Methods
        private void Chain_Initialize(ServiceProvider provider)
        {
            _provider = provider;

            this.OnChainChanged += this.Chain_HandleOnChainChanged;

            this.MaleConnectionNode.OnAttached += this.Chain_HandleMaleConnectionNodeAttached;
            this.MaleConnectionNode.OnDetached += this.Chain_HandleMaleConnectionNodeDetached;

            // If the current ship part is not within a chain, create one now.
            if (this.Chain == default(Chain) && this.IsRoot)
                Chain.Create(provider, this);
        }

        private void Chain_Release()
        {
            _provider = null;

            this.OnChainChanged -= this.Chain_HandleOnChainChanged;

            this.MaleConnectionNode.OnAttached -= this.Chain_HandleMaleConnectionNodeAttached;
            this.MaleConnectionNode.OnDetached -= this.Chain_HandleMaleConnectionNodeDetached;

            // Auto release the saved chain.
            this.Chain = null;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Primary method invoked when the current shippart's chain
        /// is updated. This will recursively update all children chains as
        /// well.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="old"></param>
        /// <param name="value"></param>
        private void Chain_HandleOnChainChanged(ShipPart sender, Chain old, Chain value)
        {
            // Continue & iterate down the chain...
            foreach(ConnectionNode female in this.FemaleConnectionNodes)
                if (female.Attached) // Update the child's chain value...
                    female.Target.Parent.Chain = value;
        }

        /// <summary>
        /// When the current shippart is connected to a chain, we should remove it from its current
        /// chain and add it to the target chain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void Chain_HandleMaleConnectionNodeAttached(ConnectionNode sender, ConnectionNode arg)
        {
            // Add into new chain...
            arg.Parent.Chain.Add(this);
        }

        /// <summary>
        /// When a ShipPart is detached from a chain we must remove it
        /// from its current chain.
        /// 
        /// Note: by default, a chain will automatically add a non root
        /// piece into a new fresh chain on removal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void Chain_HandleMaleConnectionNodeDetached(ConnectionNode sender, ConnectionNode arg)
            => Chain.Create(_provider, this);
        #endregion
    }
}