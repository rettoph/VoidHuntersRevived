using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using VoidHuntersRevived.Common.Entities.ShipParts.Extensions;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public class Tree
    {
        private readonly List<int> _nodes;

        public readonly int EntityId;
        public readonly ReadOnlyCollection<int> Nodes;
        public int? HeadId => _nodes.Any() ? _nodes[0] : null;

        public Tree(int entityId)
        {
            _nodes = new List<int>();

            this.EntityId = entityId;
            this.Nodes = new ReadOnlyCollection<int>(_nodes);
        }

        /// <summary>
        /// <para>Warning: Do not use this method.</para>
        /// <para>
        /// This should only be called by the domain level tree system as
        /// required logic to adding a new node to a tree.
        /// If you think you need to use this you probably want to publish
        /// a <see cref="Events.DestroyNode"/> event to your simulation.
        /// </para>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Node Add(Node node)
        {
            _nodes.Add(node.EntityId);

            return node;
        }

        /// <summary>
        /// <para>Warning: Do not use this method.</para>
        /// <para>
        /// This should only be called by the domain level tree system as
        /// required logic to adding a new node to a tree.
        /// If you think you need to use this you probably want to publish
        /// a <see cref="Events.DestroyNode"/> event to your simulation.
        /// </para>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Remove(Node node)
        {
            return _nodes.Remove(node.EntityId);
        }
    }
}
