using Guppy.Extensions.Collection;
using Guppy.Factories;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// Implement's ShipPart functionality specifically
    /// related to connection nodes and the overal part's
    /// chain. 
    /// </summary>
    public partial class ShipPart
    {
        #region Enums
        [Flags]
        public enum ChainUpdate
        {
            None = 0,
            Up = 1,
            Down = 2,
            Both = Up | Down
        }
        #endregion

        #region Private Fields
        private CreatableFactory<ConnectionNode> _connectionNodefactory;
        #endregion

        #region Internal Properties
        internal ChainUpdate dirty { get; private set; }
        #endregion

        #region Public Properties
        /// <summary>
        /// The parts main mail connection node. All ShipParts must contain a male node.
        /// </summary>
        public ConnectionNode MaleConnectionNode { get; private set; }

        /// <summary>
        /// A list of the ShipPart's female nodes. This may be empty if there are no female nodes.
        /// </summary>
        public ConnectionNode[] FemaleConnectionNodes { get; private set; }

        /// <inheritdoc />
        public override Controller Controller { get => this.IsRoot ? base.Controller : this.Root.Controller; }

        public IEnumerable<ShipPart> Children { get => this.FemaleConnectionNodes.Where(fe => fe.Target != null).Select(fe => fe.Target.Parent); }
        #endregion

        #region Events
        public event EventHandler<ChainUpdate> OnChainUpdated;
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// ConnectionNode specific creation.
        /// Called withing ShipPart.Core.cs
        /// </summary>
        private void ConnectionNode_Create(IServiceProvider provider)
        {
            _connectionNodefactory = provider.GetRequiredService<CreatableFactory<ConnectionNode>>();
        }

        /// <summary>
        /// ConnectionNode specific initialization.
        /// Called withing ShipPart.Core.cs
        /// </summary>
        private void ConnectionNode_Initialize()
        {
            // Build and configure connection node instances
            this.MaleConnectionNode = _connectionNodefactory.Build<ConnectionNode>(node => node.Configure(-1, this, this.Configuration.MaleConnectionNode));
            this.FemaleConnectionNodes = this.Configuration.FemaleConnectionNodes
                .Select((female_config, idx) => _connectionNodefactory.Build<ConnectionNode>(node => node.Configure(idx, this, female_config)))
                .ToArray();

            // Mark the current ShipPart as dirty by default
            this.dirty = ChainUpdate.Down;

            this.OnCleaned += this.ConnectionNode_HandleClean;
            this.MaleConnectionNode.OnAttached += this.ConnectionNode_HandleMaleConnectionNodeAttached;
            this.MaleConnectionNode.OnDetached += this.ConnectionNode_HandleMaleConnectionNodeDetached;
        }

        private void ConnectionNode_Dispose()
        {
            this.OnCleaned -= this.ConnectionNode_HandleClean;
            this.MaleConnectionNode.OnAttached -= this.ConnectionNode_HandleMaleConnectionNodeAttached;
            this.MaleConnectionNode.OnDetached -= this.ConnectionNode_HandleMaleConnectionNodeDetached;

            this.MaleConnectionNode.Dispose();
            this.FemaleConnectionNodes.ForEach(f => {
                if (f.Attached)
                {
                    // Add any children into the current controller...
                    this.Controller.Add(f.Target.Parent);
                    f.Detach();
                }

                f.Dispose();
            });
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Immediately clean down the current ShipPart's chain
        /// in all specified directions.
        /// </summary>
        /// <param name="direction"></param>
        private void CleanChain(ChainUpdate directions)
        {
            this.OnChainUpdated?.Invoke(this, directions);

            // Recusively update all elements up the chain
            if (directions.HasFlag(ChainUpdate.Up) && !this.IsRoot)
                this.Parent.CleanChain(ChainUpdate.Up);
            // Recursively update all elements down the chain
            if (directions.HasFlag(ChainUpdate.Down))
                for (Int32 i = 0; i < this.FemaleConnectionNodes.Length; i++)
                    this.FemaleConnectionNodes[i].Target?.Parent.CleanChain(ChainUpdate.Down);

            // Mark the internal chain as clean
            this.dirty &= ~directions;
        }

        public void GetOpenFemaleConnectionNodes(ref List<ConnectionNode> list)
        {
            foreach (ConnectionNode female in this.FemaleConnectionNodes)
                if (female.Attached)
                    female.Target.Parent.GetOpenFemaleConnectionNodes(ref list);
                else
                    list.Add(female);
        }
        #endregion

        #region Event Handlers
        private void ConnectionNode_HandleClean(object sender, GameTime arg)
        {
            if (this.dirty != ChainUpdate.None)
                this.CleanChain(this.dirty);
        }

        private void ConnectionNode_HandleMaleConnectionNodeAttached(Object sender, ConnectionNode arg)
        {
            // Instantly clean the current chain
            this.dirty = ChainUpdate.Both;
            this.SetDirty(true);

            // Setup the controler body..
            this.Root.Controller.SetupBody(this, this.Body);

            // Auto add the ship part into the annex
            this.annex.Add(this);
        }

        private void ConnectionNode_HandleMaleConnectionNodeDetached(Object sender, ConnectionNode arg)
        {
            // Update the Body's world position
            this.SetWorldTransform(arg.Parent.Root, this.Body);

            // Mark the current & old parent chain dirty
            this.dirty = ChainUpdate.Down;
            this.SetDirty(true);
            arg.Parent.dirty = ChainUpdate.Up;
            arg.Parent.SetDirty(true);
        }
        #endregion

        #region Network Methods
        private void ConnectionNode_Read(NetIncomingMessage im)
        {
            if (im.ReadBoolean())
            { // Read the attachment data...
                this.MaleConnectionNode.Attach(im.ReadEntity<ShipPart>(this.entities).FemaleConnectionNodes[im.ReadInt32()]);
            }
        }

        private void ConnectionNode_Write(NetOutgoingMessage om)
        {
            if (om.WriteIf(this.MaleConnectionNode.Attached))
            { // Write the attachment data if there is a connection
                om.Write(this.MaleConnectionNode.Target.Parent);
                om.Write(this.MaleConnectionNode.Target.Id);
            }
        }
        #endregion
    }
}
