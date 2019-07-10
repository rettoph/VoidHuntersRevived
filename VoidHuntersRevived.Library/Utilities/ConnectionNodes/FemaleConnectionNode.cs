using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.Configurations;
using Guppy.Extensions.DependencyInjection;
using Guppy.Loaders;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Utilities.ConnectionNodes
{
    public class FemaleConnectionNode : ConnectionNode
    {
        public FemaleConnectionNode(Int32 id, ShipPart parent, float rotation, Vector2 position, ContentLoader content, SpriteBatch spriteBatch = null) : base(id, parent, rotation, position, spriteBatch)
        {
            this.texture = content.Get<Texture2D>("texture:connection-node:female");
        }

        #region Connection Methods
        /// <summary>
        /// Connect the input male node to the current female node
        /// </summary>
        /// <param name="target"></param>
        protected internal void Attatch(ShipPart target)
        {
            // First, ensure the the input node does not belong to the sameparent as the current node
            if (this.Parent == target.Parent)
                throw new Exception("Unable to attatch to male node, both nodes are siblings.");

            
            if (this.Target != null) // Auto detatch the current node if it is connected to something else.
                this.Detatch();
            if (target.MaleConnectionNode.Target != null) // Auto detatch the target node if it is connected to something else.
                (target.MaleConnectionNode.Target as FemaleConnectionNode).Detatch();

            this.Target = target.MaleConnectionNode;
            this.Target.Target = this;

            // Alert the current node's parent to remap its connection nodes
            this.Parent.RemapConnectioNodes();
            this.Parent.Root.RemapConnectioNodes(deep: false);
        }

        protected internal void Detatch()
        {
            // Update the old target's new chain
            this.Target.Target = null;
            this.Target.Parent.RemapConnectioNodes();

            // Update the current chain
            this.Target = null;
            this.Parent.RemapConnectioNodes();
            this.Parent.Root.RemapConnectioNodes(deep: false);
        }
        #endregion
    }
}
