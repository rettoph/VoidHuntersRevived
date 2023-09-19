using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Descriptors;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface IPieceService
    {
        Piece GetByKey(string key);

        bool TryGetByKey(string key, [MaybeNullWhen(false)] out Piece piece);

        Piece[] All<TDescriptor>()
            where TDescriptor : PieceDescriptor;

        Piece[] All();
    }
}
