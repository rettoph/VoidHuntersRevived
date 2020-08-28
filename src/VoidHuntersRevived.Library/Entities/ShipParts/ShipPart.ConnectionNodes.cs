using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Interfaces;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Enums;
using Guppy.Collections;
using Guppy.IO.Extensions.log4net;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// Partial implementation of ShipPart that
    /// specializes on all things related to connection
    /// nodes.
    /// </summary>
    public partial class ShipPart
    {
        #region Enums
        [Flags]
        public enum DirtyChainType
        {
            None = 0,
            Up = 1,
            Down = 2,
            Both = Up | Down
        }
        #endregion

        #region Private Fields
        private ShipPart _root;
        #endregion

        #region Public Attributes
        /// <summary>
        /// The parts main mail connection node. All ShipParts must contain a male node.
        /// </summary>
        public ConnectionNode MaleConnectionNode { get; private set; }

        /// <summary>
        /// A list of the ShipPart's female nodes. This may be empty if there are no female nodes.
        /// </summary>
        public ConnectionNode[] FemaleConnectionNodes { get; private set; }

        /// <summary>
        /// A list of all children downstream connected directly to
        /// the current ShipPart.
        /// </summary>
        public IEnumerable<ShipPart> Children { get => this.FemaleConnectionNodes.Where(fe => fe.Target != null).Select(fe => fe.Target.Parent); }
        #endregion

        #region Events
        public event GuppyEventHandler<ShipPart, DirtyChainType> OnChainCleaned;
        public event GuppyDeltaEventHandler<ShipPart, ShipPart> OnRootChanged;
        #endregion

        #region Lifecycle Methods
        private void ConnectionNode_Initialize(ServiceProvider provider)
        {
            // Create new connection nodes for the internal element
            this.MaleConnectionNode = ConnectionNode.Build(provider, this.Configuration.MaleConnectionNode, this, -1);
            this.FemaleConnectionNodes = this.Configuration.FemaleConnectionNodes.Select((f, i) =>
                ConnectionNode.Build(provider, f, this, i)).ToArray();

            this.MaleConnectionNode.OnAttached += this.ConnectionNode_HandleMaleConnectionNodeAttached;
            this.MaleConnectionNode.OnDetached += this.ConnectionNode_HandleMaleConnectionNodeDetached;
        }

        private void ConnectionNode_Dispose()
        {
            this.MaleConnectionNode.TryDispose();
            this.FemaleConnectionNodes.ForEach(f => f.TryDispose());

            this.MaleConnectionNode.OnAttached -= this.ConnectionNode_HandleMaleConnectionNodeAttached;
            this.MaleConnectionNode.OnDetached -= this.ConnectionNode_HandleMaleConnectionNodeDetached;
        }
        #endregion

        #region Helper Methods
        public void CleanChain(DirtyChainType direction)
        {
            this.log.Verbose(() => $"Cleaning ({this.Id}) => {direction}");

            // Recursively update all elements down the chain
            if (direction.HasFlag(DirtyChainType.Down))
            { // If the chain is cleaning down...
                // Check the cached root value
                if(this.Root != _root)
                { // If the root has changed...
                    this.log.Verbose(() => $"ShipPart({this.Id}) Root changed from ShipPart({_root?.Id}) to ShipPart({this.Root.Id})");
                    this.OnRootChanged?.Invoke(this, _root, this.Root);
                    _root = this.Root;
                }

                // Continue & iterate down the chain.
                this.FemaleConnectionNodes.ForEach(f =>
                {
                    if (f.Attached)
                        f.Target.Parent.CleanChain(DirtyChainType.Down);
                });
            }

            // Recuersively update all elements within the chain.
            if (direction.HasFlag(DirtyChainType.Up) && !this.IsRoot)
                this.Parent.CleanChain(DirtyChainType.Up);

            // Invoke the internal method...
            this.OnChainCleaned?.Invoke(this, direction);
        }

        public void GetOpenFemaleConnectionNodes(ref IList<ConnectionNode> list)
        {
            foreach (ConnectionNode female in this.FemaleConnectionNodes)
                if (female.Attached)
                    female.Target.Parent.GetOpenFemaleConnectionNodes(ref list);
                else
                    list.Add(female);
        }
        #endregion

        #region Event Handlers
        private void ConnectionNode_HandleMaleConnectionNodeAttached(ConnectionNode sender, ConnectionNode arg)
        {
            this.log.Verbose(() => $"Attached ShipPart({this.Id}) to ShipPart({this.MaleConnectionNode.Target.Parent.Id}) FemaleNode({this.MaleConnectionNode.Target.Id})");

            this.CleanChain(DirtyChainType.Both);
        }

        private void ConnectionNode_HandleMaleConnectionNodeDetached(ConnectionNode sender, ConnectionNode arg)
        {
            this.CleanChain(DirtyChainType.Down);
            arg.Parent.CleanChain(DirtyChainType.Up);
        }
        #endregion
    }
}
