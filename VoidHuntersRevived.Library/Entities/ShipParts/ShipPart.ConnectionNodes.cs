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
        public MaleConnectionNode MaleConnectionNode { get; private set; }
        public IEnumerable<FemaleConnectionNode> FemaleConnectionNodes { get; private set; }
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
            this.MaleConnectionNode = factory.Build<MaleConnectionNode>(node => node.Configure(1, _configuration.MaleConnectionNode));
            this.FemaleConnectionNodes = _configuration.FemaleConnectionNodes.Select((female_config, idx) => factory.Build<FemaleConnectionNode>(node => node.Configure(idx, female_config)));
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
    }
}
