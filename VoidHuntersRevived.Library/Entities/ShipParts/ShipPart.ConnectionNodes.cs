using GalacticFighters.Library.Entities.ShipParts.ConnectionNodes;
using GalacticFighters.Library.Factories;
using Guppy.Extensions.Collection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticFighters.Library.Entities.ShipParts
{
    /// <summary>
    /// Contains connection node specific code.
    /// </summary>
    public partial class ShipPart
    {
        #region Public Attributes
        /// <summary>
        /// The parts main mail connection node. All ShipParts must contain a male node.
        /// </summary>
        public MaleConnectionNode MaleConnectionNode { get; private set; }

        /// <summary>
        /// A list of the ShipPart's female nodes. This may be empty if there are no female nodes.
        /// </summary>
        public FemaleConnectionNode[] FemaleConnectionNodes { get; private set; }

        /// <summary>
        /// The current ShipPart's immediate parent, if any.
        /// </summary>
        public ShipPart Parent { get { return this.MaleConnectionNode.Target?.Parent; } }

        /// <summary>
        /// Wether or not the current ShipPart is 
        /// </summary>
        public Boolean IsRoot { get { return !this.MaleConnectionNode.Attached; } }

        /// <summary>
        /// Return the root most ShipPart in the current ShipPart's chain.
        /// </summary>
        public ShipPart Root { get { return this.IsRoot ? this : this.Parent.Root; } }
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// ConnectionNode specific initialization.
        /// Called withing ShipPart.Core.cs
        /// </summary>
        private void ConnectionNode_PreInitialize()
        {
            // Load the connection node factory
            var factory = this.provider.GetRequiredService<ConnectionNodeFactory>();

            // Build and configure connection node instances
            this.MaleConnectionNode = factory.Build<MaleConnectionNode>(node => node.Configure(-1, this, this.config.MaleConnectionNode));
            this.FemaleConnectionNodes = this.config.FemaleConnectionNodes
                .Select((female_config, idx) => factory.Build<FemaleConnectionNode>(node => node.Configure(idx, this, female_config)))
                .ToArray();

            // Bind event listeners
            this.MaleConnectionNode.Events.TryAdd<ConnectionNode>("attached", this.HandleMaleConnectionNodeAttached);
        }

        /// <summary>
        /// Dispose of the internal connection node data.
        /// Called withing ShipPart.Core.cs
        /// </summary>
        private void ConnectionNode_Dispose()
        {
            this.MaleConnectionNode.Dispose();
            this.FemaleConnectionNodes.ForEach(female => female.Dispose());
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Used to update the connection nodes and chain placement of a specific
        /// part. This is recursively called on all children belonging to a specific
        /// ShipPart
        /// </summary>
        private void RemapConnectionNodes()
        {
            // Update the current parts translation...
            this.UpdateLocalTranslation();

            // Update the current ShipPart's chain placement...
            this.UpdateChainPlacement();

            // Recursively call is for all internal female nodes
            this.FemaleConnectionNodes.ForEach(female => female.Target?.Parent.RemapConnectionNodes());
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the male connection node is attached, we must remape the connection nodes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleMaleConnectionNodeAttached(object sender, ConnectionNode arg)
        {
            this.RemapConnectionNodes();
        }
        #endregion
    }
}
