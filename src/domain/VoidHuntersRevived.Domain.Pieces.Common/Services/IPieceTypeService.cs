using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Pieces.Common.Services
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
