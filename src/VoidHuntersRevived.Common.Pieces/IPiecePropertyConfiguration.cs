using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Pieces
{
    public delegate void InitializeComponentDelegate<T, TComponent>(PieceProperty<T> property, ref TComponent component)
        where T : class, IPieceProperty
        where TComponent : unmanaged;

    public interface IPiecePropertyConfiguration
    {
        Type Type { get; }
    }

    public interface IPiecePropertyConfiguration<T> : IPiecePropertyConfiguration
        where T : class, IPieceProperty
    {
        void RequiresComponent<TComponent>(InitializeComponentDelegate<T, TComponent> initializer)
            where TComponent : unmanaged;
    }
}
