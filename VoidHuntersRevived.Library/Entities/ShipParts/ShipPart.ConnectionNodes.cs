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
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// ConnectionNode specific creation.
        /// Called withing ShipPart.Core.cs
        /// </summary>
        private void ConnectionNode_Create(IServiceProvider provider)
        {
            _connectionNodefactory = provider.GetRequiredService<CreatableFactory<ConnectionNode>>();

            this.Events.Register<ChainUpdate>("chain:updated");
        }

        /// <summary>
        /// ConnectionNode specific initialization.
        /// Called withing ShipPart.Core.cs
        /// </summary>
        private void ConnectionNode_Initialize()
        {
            // Build and configure connection node instances
            var data = this.Configuration.GetData<ShipPartConfiguration>();
            this.MaleConnectionNode = _connectionNodefactory.Build<ConnectionNode>(node => node.Configure(-1, this, data.MaleConnectionNode));
            this.FemaleConnectionNodes = data.FemaleConnectionNodes
                .Select((female_config, idx) => _connectionNodefactory.Build<ConnectionNode>(node => node.Configure(idx, this, female_config)))
                .ToArray();

            this.Events.TryAdd<GameTime>("clean", this.ConnectionNode_HandleClean);

            // Bind event listeners tto automatically remap connection node data on a male attachment or female detachment
            this.MaleConnectionNode.Events.TryAdd<ConnectionNode>("attached", (s, n) =>
            {
                // Auto update the controller if it is not already defined...
                if (this.Controller != this.Root.Controller)
                    this.Root.Controller.Add(this);

                this.dirty |= ChainUpdate.Both;
                this.SetDirty(true);
            });
            this.MaleConnectionNode.Events.TryAdd<ConnectionNode>("detached", (s, n) =>
            {
                // Update the Body's world position
                this.SetWorldTransform(n.Parent.Root, this.Body);

                // Mark the current & old parent chain dirty
                this.dirty |= ChainUpdate.Down;
                n.Parent.dirty |= ChainUpdate.Up;
                this.SetDirty(true);
                n.Parent.SetDirty(true);
            });

            // Mark the current ShipPart as dirty by default
            this.dirty = ChainUpdate.Down;
        }

        private void ConnectionNode_Dispose()
        {
            this.MaleConnectionNode.Dispose();
            this.FemaleConnectionNodes.ForEach(f => f.Dispose());
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
            this.Events.TryInvoke<ChainUpdate>(this, "chain:updated", directions);

            // Recusively update all elements up the chain
            if (directions.HasFlag(ChainUpdate.Up) && !this.IsRoot)
                this.Parent.CleanChain(ChainUpdate.Up);
            // Recursively update all elements down the chain
            if (directions.HasFlag(ChainUpdate.Down))
                for (Int32 i = 0; i < this.FemaleConnectionNodes.Length; i++)
                    this.FemaleConnectionNodes[i].Target?.Parent.CleanChain(ChainUpdate.Down);

            // Mark the internal chain as clean
            this.dirty = ChainUpdate.None;
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

        #region Event Handler
        private void ConnectionNode_HandleClean(object sender, GameTime arg)
        {
            if (this.dirty != ChainUpdate.None)
                this.CleanChain(this.dirty);
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
