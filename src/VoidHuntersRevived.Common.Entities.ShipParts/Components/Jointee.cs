using MonoGame.Extended.Entities;
using System.Collections.ObjectModel;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    /// <summary>
    /// Can be thought of as a list of "female"
    /// connections
    /// </summary>
    public sealed class Jointee
    {
        private readonly List<Jointing> _children;

        public readonly Entity Entity;
        public readonly ReadOnlyCollection<Jointing> Children;

        public Jointee(Entity entity)
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

        public void Remove(Jointing child)
        {
            if(_children.Remove(child))
            {
                child.Joint.Jointing = null;
                child.Parent.Jointing = null;
            }
        }
    }
}
