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
    public class MaleConnectionNode : ConnectionNode
    {
        public MaleConnectionNode(ShipPart parent, float rotation, Vector2 position, ContentLoader content, SpriteBatch spriteBatch = null) : base(-1, parent, rotation, position, spriteBatch)
        {
            this.texture = content.Get<Texture2D>("texture:connection-node:male");
        }

        #region Connection Methods
        /// <summary>
        /// Connect the input female node to the current male node
        /// </summary>
        /// <param name="target"></param>
        protected internal void AttatchTo(FemaleConnectionNode target)
        {
            // First, ensure the the input node does not belong to the sameparent as the current node
            if (this.Parent == target.Parent)
                throw new Exception("Unable to attatch to female node, both nodes are siblings.");
            else if (target.Target != null)
                throw new Exception("Unable to attatch to female node, target already bound to another node.");

            // Auto detatch the current male node, if it is connected to something else.
            if (this.Target != null)
                this.Detatch();

            this.Target = target;
            this.Target.Target = this;

            // Alert the new chain to remap its connection nodes
            this.Parent.RemapConnectioNodes();
        }

        protected internal void Detatch()
        {
            this.Target.Target = null;
            this.Target = null;

            // Alert the new chain to remap its connection nodes
            this.Parent.RemapConnectioNodes();
        }
        #endregion
    }
}
