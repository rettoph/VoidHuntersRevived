using GalacticFighters.Library.Entities.ShipParts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Utilities.ConnectionNodes
{
    public class MaleConnectionNode : ConnectionNode
    {
        public event EventHandler<FemaleConnectionNode> OnAttatchedTo;
        public event EventHandler OnDetachedFrom;

        public MaleConnectionNode(ShipPart parent, float rotation, Vector2 position) : base(-1, parent, rotation, position)
        {
        }

        #region Connection Methods
        /// <summary>
        /// Connect the input male node to the current female node
        /// </summary>
        /// <param name="target"></param>
        protected internal void AttatchTo(FemaleConnectionNode target)
        {
            // First, ensure the the input node does not belong to the sameparent as the current node
            if (this.Parent == target.Parent)
                throw new Exception("Unable to attatch to male node, both nodes are siblings.");


            if (this.Target != null) // Auto detatch the current node if it is connected to something else.
                this.DetatchFrom();
            if (target.Target != null) // Auto detatch the target node if it is connected to something else.
                (target.Target as MaleConnectionNode).DetatchFrom();

            this.Target = target;
            this.Target.Target = this;

            // Alert the current node's parent to remap its connection nodes
            this.Parent.RemapConnectioNodes();
            this.Parent.Root.RemapConnectioNodes(deep: false);

            // Trigger the attached event
            this.OnAttatchedTo?.Invoke(this, this.Target as FemaleConnectionNode);
        }

        protected internal void DetatchFrom()
        {
            if (this.Target != null)
            { // Only do anything if the ship-part is attached to anything to begin with
                // Update the old target's new chain
                this.Target.Target = null;
                this.Target.Parent.RemapConnectioNodes();
                this.Target.Parent.Root.RemapConnectioNodes(deep: false);

                // Update the current chain
                this.Target = null;
                this.Parent.RemapConnectioNodes();
                this.Parent.Root.RemapConnectioNodes(deep: false);

            }

            // Trigger the detach event
            this.OnDetachedFrom?.Invoke(this, null);
        }
        #endregion
    }
}
