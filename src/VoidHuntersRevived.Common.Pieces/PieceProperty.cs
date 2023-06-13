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
        public readonly IPieceProperty Property;

        public PieceProperty(IPieceProperty property)
        {
            this.Property = property;
        }
    }

    public class PieceProperty<T> : PieceProperty
        where T : class, IPieceProperty
    {
        public new readonly T Property;
        public readonly PiecePropertyId<T> Id;

        public PieceProperty(T property, PiecePropertyId<T> id) : base(property)
        {
            this.Property = property;
            this.Id = id;
        }
    }
}
