using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.Collections;
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
        #endregion

        #region Public Properties
        public Chain Chain
        {
            get => _chain;
            internal set => this.OnChainChanged.InvokeIfChanged(value != _chain, this, ref _chain, value);
        }
        #endregion

        #region Events
        public OnChangedEventDelegate<ShipPart, Chain> OnChainChanged;
        #endregion

        #region Lifecycle Methods
        private void Chain_Initialize(ServiceProvider provider)
        {
            this.OnChainChanged += this.Chain_HandleOnChainChanged;

            this.MaleConnectionNode.OnAttached += this.Chain_HandleMaleConnectionNodeAttached;
            this.MaleConnectionNode.OnDetached += this.Chain_HandleMaleConnectionNodeDetached;

            // If the current ship part is not within a chain, create one now.
            if (this.Chain == default(Chain) && this.IsRoot)
                Chain.Create(provider, this);
        }

        private void Chain_Release()
        {
            this.OnChainChanged -= this.Chain_HandleOnChainChanged;

            this.MaleConnectionNode.OnAttached -= this.Chain_HandleMaleConnectionNodeAttached;
            this.MaleConnectionNode.OnDetached -= this.Chain_HandleMaleConnectionNodeDetached;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Primary method invoked when the chain current shippart's chain
        /// is updated. This will recursively update all children chains as
        /// well.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="old"></param>
        /// <param name="value"></param>
        private void Chain_HandleOnChainChanged(ShipPart sender, Chain old, Chain value)
        {
            // Continue & iterate down the chain...
            this.FemaleConnectionNodes.ForEach(f =>
            {
                if (f.Attached) // Update the child's chain value...
                    f.Target.Parent.Chain = value;
            });
        }

        /// <summary>
        /// When the current shippart is connected to a chain, we should remove it from its current
        /// chain and add it to the target chain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void Chain_HandleMaleConnectionNodeAttached(ConnectionNode sender, ConnectionNode arg)
        {
            // Remove from old chain (if any)...
            this.Chain.Remove(this);

            // Add into new chain...
            arg.Parent.Root.Chain.Add(this);
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
        {
            this.Chain.Remove(this);
        }
        #endregion
    }
}