using MonoGame.Extended.Entities;
using System.Collections.ObjectModel;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    /// <summary>
    /// Can be thought of as a list of "female"
    /// connections
    /// </summary>
    public sealed class Jointings
    {
        private readonly List<Jointing> _children;

        public readonly Entity Entity;
        public readonly ReadOnlyCollection<Jointing> Children;

        internal Jointings(Entity entity)
        {
            _children = new List<Jointing>();

            this.Entity = entity;
            this.Children = new ReadOnlyCollection<Jointing>(_children);
        }

        public void Add(Jointing child)
        {
            child.Joint.Jointing = child;
            child.Parent.Jointing = child;

            _children.Add(child);
        }
    }
}
