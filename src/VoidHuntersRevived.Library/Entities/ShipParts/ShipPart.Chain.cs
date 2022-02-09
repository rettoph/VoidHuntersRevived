using Guppy.EntityComponent.DependencyInjection;
using Guppy.Events.Delegates;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart
    {
        #region Private Fields
        private Chain _chain;
        #endregion

        #region Public Properties
        /// <summary>
        /// The current ShipPart's <see cref="WorldObjects.Chain"/>, if any.
        /// </summary>
        public Chain Chain
        {
            get => _chain;
            internal set => this.OnChainChanged.InvokeIf(_chain != value, this, ref _chain, value);
        }
        #endregion

        #region Events
        /// <summary>
        /// Automatically invoked when the <see cref="ShipPart.Chain"/> value is updated.
        /// </summary>
        public event OnChangedEventDelegate<ShipPart, Chain> OnChainChanged;
        #endregion

        #region Lifecycle Methods
        private void Chain_Initialize(ServiceProvider provider)
        {
            this.PostTreeClean += this.Chain_HandlePostTreeCleaned;
            this.OnChainChanged += this.Chain_HandleChainChanged;
        }

        private void Chain_Uninitialize()
        {
            this.Chain = default;

            this.OnChainChanged -= this.Chain_HandleChainChanged;
            this.PostTreeClean -= this.Chain_HandlePostTreeCleaned;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Automatically pass a chain changed event data down the tree.
        /// TODO: Is there a way to hijack the <see cref="CleanTree(Enums.TreeComponent)"/>
        /// method for this?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="old"></param>
        /// <param name="value"></param>
        private void Chain_HandleChainChanged(ShipPart sender, Chain old, Chain value)
        {
            foreach (ConnectionNode node in this.ConnectionNodes)
                if (node.Connection.State == ConnectionNodeState.Parent)
                    node.Connection.Target.Owner.Chain = value;
        }

        /// <summary>
        /// After a tree has completed cleaning we should update the internal chain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="source"></param>
        /// <param name="components"></param>
        private void Chain_HandlePostTreeCleaned(ShipPart sender, ShipPart source, TreeComponent components)
        {
            if((components & TreeComponent.Node) != 0 && !this.IsRoot && this == source)
            {
                this.Chain = this.Root.Chain;
            }
        }
        #endregion
    }
}
