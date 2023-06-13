using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces
{
    public class PieceProperty
    {
        public readonly Type Type;
        public readonly IPieceProperty Property;

        public PieceProperty(Type type, IPieceProperty property)
        {
            this.Type = type;
            this.Property = property;
        }
    }

    public class PieceProperty<T> : PieceProperty
        where T : class, IPieceProperty
    {
        public new readonly T Property;
        public readonly Piece<T> Component;

        public PieceProperty(T property, Piece<T> id) : base(typeof(T), property)
        {
            this.Property = property;
            this.Component = id;
        }
    }
}
