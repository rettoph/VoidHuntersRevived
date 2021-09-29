using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Contexts.Utilities;
using System.Linq;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart
    {
        #region Public Properties
        /// <summary>
        /// Each ShipPart may only have a single <see cref="ConnectionNode"/> with a
        /// <see cref="ConnectionNode.State"/> value of <see cref="ConnectionNodeState.Child"/>
        /// at a time. That node will be stored here.
        /// </summary>
        public ConnectionNode ChildConnectionNode { get; private set; }

        /// <summary>
        /// Zero or more <see cref="ConnectionNode"/>s, with which other <see cref="ShipPart"/>s
        /// may attach to.
        /// </summary>
        public ConnectionNode[] ConnectionNodes { get; private set; }

        /// <summary>
        /// Indicates whether or not the current <see cref="ShipPart"/>
        /// is the root most part of its current tree.
        /// </summary>
        public Boolean IsRoot { get; private set; }

        /// <summary>
        /// The Root of the <see cref="ShipPart"/>'s current tree.
        /// This may be the current <see cref="ShipPart"/> itself.
        /// </summary>
        public ShipPart Root { get; private set; }

        /// <summary>
        /// The <see cref="ShipPart"/>'s immidate parent if any.
        /// If <see cref="IsRoot"/>, this will be null.
        /// </summary>
        public ShipPart Parent { get; private set; }
        #endregion

        #region Events
        public delegate void TreeCleanDelegate(ShipPart sender, ShipPart source, TreeComponent components);

        /// <summary>
        /// Invoked when the <see cref="ShipPart"/>'s tree has recieved a clean request.
        /// </summary>
        public event TreeCleanDelegate PreTreeClean;

        /// <summary>
        /// Invoked when the <see cref="ShipPart"/>'s tree has recieved a clean request.
        /// </summary>
        public event TreeCleanDelegate OnTreeClean;

        /// <summary>
        /// Invoked when the <see cref="ShipPart"/>'s tree has recieved a clean request.
        /// </summary>
        public event TreeCleanDelegate PostTreeClean;
        #endregion

        #region Lifecycle Methods
        private void Tree_Create(GuppyServiceProvider provider)
        {

        }

        private void Tree_Initialize(GuppyServiceProvider provider)
        {
            // Construct an array of ConnectionNodes based on the defined ShipPart context.
            Int32 nodeIndex = 0;
            this.ConnectionNodes = this.Context.ConnectionNodes.Select(nodeDto =>
            {
                ConnectionNode node = ConnectionNode.Build(provider, nodeDto, this, nodeIndex++);

                node.OnConnectionChanged += this.HandleConnectionNodeConnectionChanged;

                return node;
            }).ToArray();
        }

        private void Tree_PostInitialize(GuppyServiceProvider provider)
        {
            this.CleanTree(TreeComponent.Node);
        }

        private void Tree_Release()
        {
            foreach (ConnectionNode node in this.ConnectionNodes)
            {
                node.OnConnectionChanged -= this.HandleConnectionNodeConnectionChanged;

                node.TryRelease();
            };

            this.ConnectionNodes = new ConnectionNode[0];
            this.ChildConnectionNode = default;
        }

        private void Tree_Dispose()
        {
        }
        #endregion

        #region Event Handlers
        private void HandleConnectionNodeConnectionChanged(ConnectionNode sender, ConnectionNodeConnection old, ConnectionNodeConnection value)
        {
            if (old.State == ConnectionNodeState.Child)
            { // The old child node has been released.
                this.ChildConnectionNode = default;
                this.CleanTree(TreeComponent.Node | TreeComponent.Children);
            }
            else if(old.State == ConnectionNodeState.Parent)
            { // The old parent node has been released.
                this.CleanTree(old.Target.Owner, TreeComponent.Parent);
            }
            else if (value.State == ConnectionNodeState.Child)
            { // A new child node has been defined.
                if (this.ChildConnectionNode != default)
                    throw new Exception("Unable to create mutiple child ConnectionNodes within a single ShipPart.");

                // Update the child connection node value.
                this.ChildConnectionNode = sender;
                this.CleanTree(TreeComponent.Node | TreeComponent.Parent | TreeComponent.Children);
            }
        }

        /// <summary>
        /// Update the internal parts of the current tree.
        /// </summary>
        protected internal void CleanTree(TreeComponent components)
        {
            this.CleanTree(this, components);
        }

        /// <summary>
        /// Update the internal parts of the current tree. This should
        /// only ever be called automatically. Please call
        /// <see cref="CleanTree(TreeComponent)"/> to start a tree cleaning.
        /// </summary>
        private void CleanTree(ShipPart source, TreeComponent components)
        {
            this.PreTreeClean?.Invoke(this, source, components);

            // Update the current ShipPart node's positional info within the tree.
            if ((components & TreeComponent.Node) != 0)
            {
                this.IsRoot = this.ChildConnectionNode == default;
                this.Root = this.IsRoot ? this : this.ChildConnectionNode.Connection.Target.Owner.Root;
                this.Parent = this.IsRoot ? default : this.ChildConnectionNode.Connection.Target.Owner;
            }

            // Push the clean tree event up the tree first.
            // This will create a cascade call of the OnTreeCleaned event
            // down into the current node.
            if ((components & TreeComponent.Parent) != 0 && !this.IsRoot)
                this.Root.CleanTree(source, TreeComponent.Parent);

            // At this point we may invoke the cleaned event.
            // Notice we invoke this before pushing the event down the tree.
            // This is so that children recieving the event know their parents have
            // already been updated.
            this.OnTreeClean?.Invoke(this, source, components);

            // Push the clean tree event down the entire tree,
            // continuing the cascade effect but spreading
            // through all children from this point down.
            if ((components & TreeComponent.Children) != 0)
                foreach (ConnectionNode node in this.ConnectionNodes)
                    if (node.Connection.State == ConnectionNodeState.Parent)
                        node.Connection.Target.Owner.CleanTree(source, components & ~TreeComponent.Parent);

            this.PostTreeClean?.Invoke(this, source, components);
        }

        public IEnumerable<ShipPart> GetChildren()
        {
            yield return this;

            foreach(ConnectionNode node in this.ConnectionNodes)
            {
                if(node.Connection.State == ConnectionNodeState.Parent)
                {
                    foreach (ShipPart child in node.Connection.Target.Owner.GetChildren())
                    {
                        yield return child;
                    }
                }
            }
        }
        #endregion
    }
}
