using GalacticFighters.Library.Entities.ShipParts.ConnectionNodes;
using GalacticFighters.Library.Factories;
using Guppy.Extensions.Collection;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Collections;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;

namespace GalacticFighters.Library.Entities.ShipParts
{
    /// <summary>
    /// Contains connection node specific code.
    /// </summary>
    public partial class ShipPart
    {
        #region Enums
        [Flags]
        public enum ChainUpdate
        {
            Up = 1,
            Down = 2,
            Both = Up | Down
        }
        #endregion

        #region Private Fields
        private ConnectionNodeFactory _connectionNodefactory;
        #endregion

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
        private void ConnectionNode_Create(IServiceProvider provider)
        {
            _connectionNodefactory = provider.GetRequiredService<ConnectionNodeFactory>();

            this.Events.Register<ChainUpdate>("chain:updated");
        }

        /// <summary>
        /// ConnectionNode specific initialization.
        /// Called withing ShipPart.Core.cs
        /// </summary>
        private void ConnectionNode_PreInitialize()
        {
            // Build and configure connection node instances
            this.MaleConnectionNode = _connectionNodefactory.Build<MaleConnectionNode>(node => node.Configure(-1, this, this.config.MaleConnectionNode));
            this.FemaleConnectionNodes = this.config.FemaleConnectionNodes
                .Select((female_config, idx) => _connectionNodefactory.Build<FemaleConnectionNode>(node => node.Configure(idx, this, female_config)))
                .ToArray();

            // Bind event listeners tto automatically remap connection node data on a male attachment or female detachment
            this.MaleConnectionNode.Events.TryAdd<ConnectionNode>("attached", (s, n) => this.UpdateChain(ChainUpdate.Both));
            this.MaleConnectionNode.Events.TryAdd<ConnectionNode>("detached", (s, n) =>
            {
                // Rigid snap the part's body to where the fixture used to be
                this.body.Position = n.Parent.Root.Position + Vector2.Transform(Vector2.Zero, this.LocalTransformation * Matrix.CreateRotationZ(n.Parent.Root.Rotation));

                this.UpdateChain(ChainUpdate.Down); // Update down the current chain
                n.Parent.UpdateChain(ChainUpdate.Up); // Update up the old chain
            });
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
        /// Recersively trigger the UpdateChain event and recersively call
        /// the same method on all children/parents as defined in the ChainUpdate
        /// parameter
        /// </summary>
        /// <param name="directions"></param>
        internal void UpdateChain(ChainUpdate directions)
        {
            this.Events.TryInvoke<ChainUpdate>(this, "chain:updated", directions);

            // Recusively update all elements up the chain
            if (directions.HasFlag(ChainUpdate.Up) && !this.IsRoot)
                this.Parent.UpdateChain(ChainUpdate.Up);
            // Recursively update all elements down the chain
            if (directions.HasFlag(ChainUpdate.Down))
                for (Int32 i = 0; i < this.FemaleConnectionNodes.Length; i++)
                    this.FemaleConnectionNodes[i].Target?.Parent.UpdateChain(ChainUpdate.Down);

        }
        #endregion

        #region Network Methods
        private void ConnectionNode_Read(NetIncomingMessage im)
        {
            if(im.ReadBoolean())
            { // Read the attachment data...
                this.MaleConnectionNode.Attach(this.entities.GetById<ShipPart>(im.ReadGuid()).FemaleConnectionNodes[im.ReadInt32()]);
            }
        }

        private void ConnectionNode_Write(NetOutgoingMessage om)
        {
            if(om.WriteIf(this.MaleConnectionNode.Attached))
            { // Write the attachment data if there is a connection
                om.Write(this.MaleConnectionNode.Target.Parent.Id);
                om.Write(this.MaleConnectionNode.Target.Id);
            }
        }


        internal void GetOpenFemaleConnectionNodes(ref List<FemaleConnectionNode> list)
        {
            foreach (FemaleConnectionNode female in this.FemaleConnectionNodes)
                if (female.Attached)
                    female.Target.Parent.GetOpenFemaleConnectionNodes(ref list);
                else
                    list.Add(female);
        }
        #endregion
    }
}