using System.Diagnostics.CodeAnalysis;
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
