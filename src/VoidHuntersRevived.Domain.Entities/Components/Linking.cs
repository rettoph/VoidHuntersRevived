using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    /// <summary>
    /// Can be thought of as a list of "female"
    /// connections
    /// </summary>
    public sealed class Linking
    {
        private readonly List<Linked> _children;

        public readonly Entity Entity;
        public readonly ReadOnlyCollection<Linked> Children;

        internal Linking(Entity entity)
        {
            _children = new List<Linked>();
            this.Entity = entity;
            this.Children = new ReadOnlyCollection<Linked>(_children);
        }

        public void Add(Linked child)
        {
            _children.Add(child);
        }
    }
}
