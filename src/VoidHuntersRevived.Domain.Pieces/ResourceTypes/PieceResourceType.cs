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
    internal class PieceResourceType : SimpleResourceType<PieceType>
    {
        private readonly IFileService _files;

        public override string Name => "PieceType";

        public PieceResourceType(IFileService files)
        {
            _files = files;
        }

        protected override bool TryResolve(Resource<PieceType> resource, string root, string input, out PieceType value)
        {
            IFile<PieceType> piece = _files.Get<PieceType>(
                FileType.Source,
                DirectoryHelper.Combine(root, input),
                true);

            value = piece.Value;
            return piece.Success;
        }
    }
}
