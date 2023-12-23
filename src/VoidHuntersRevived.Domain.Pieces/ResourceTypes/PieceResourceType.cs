using Guppy.Attributes;
using Guppy.Files;
using Guppy.Files.Enums;
using Guppy.Files.Helpers;
using Guppy.Files.Services;
using Guppy.Resources;
using Guppy.Resources.ResourceTypes;
using VoidHuntersRevived.Common.Pieces;

namespace VoidHuntersRevived.Domain.Pieces.ResourceTypes
{
    [AutoLoad]
    internal class PieceResourceType : SimpleResourceType<Piece>
    {
        private readonly IFileService _files;

        public PieceResourceType(IFileService files)
        {
            _files = files;
        }

        protected override bool TryResolve(Resource<Piece> resource, string root, string input, out Piece value)
        {
            IFile<Piece> piece = _files.Get<Piece>(
                FileType.Source,
                DirectoryHelper.Combine(root, input),
                true);

            value = piece.Value;
            return piece.Success;
        }
    }
}
