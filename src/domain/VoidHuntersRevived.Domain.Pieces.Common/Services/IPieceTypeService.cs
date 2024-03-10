using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Pieces.Descriptors;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface IPieceTypeService
    {
        PieceType GetByKey(string key);

        bool TryGetByKey(string key, [MaybeNullWhen(false)] out PieceType piece);

        PieceType[] All<TDescriptor>()
            where TDescriptor : PieceDescriptor;

        PieceType[] All();
    }
}
