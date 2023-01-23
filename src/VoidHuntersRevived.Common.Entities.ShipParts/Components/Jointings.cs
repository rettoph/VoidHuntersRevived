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
        private readonly List<Jointed> _children;

        public readonly Entity Entity;
        public readonly ReadOnlyCollection<Jointed> Children;

        public Jointings(Entity entity)
        {
            _children = new List<Jointed>();

            this.Entity = entity;
            this.Children = new ReadOnlyCollection<Jointed>(_children);
        }

        public void Add(Jointed child)
        {
            child.Joint.Jointing = child;
            child.Parent.Jointing = child;

            _children.Add(child);
        }

        public void Remove(Jointed child)
        {
            if(_children.Remove(child))
            {
                child.Joint.Jointing = null;
                child.Parent.Jointing = null;
            }
        }
    }
}
