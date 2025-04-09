using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Assets.Common;
using Guppy.Core.Assets.Common.AssetTypes;
using VoidHuntersRevived.Domain.Graphics;
using VoidHuntersRevived.Domain.Graphics.Common;

namespace VoidHuntersRevived.Domain.Pieces.AssetTypes
{
    public class PrimitiveTypeAssetType(IFileService files) : SimpleAssetType<IPrimitiveType>
    {
        private readonly IFileService _files = files;

        public override string Name => nameof(PrimitiveType);

        protected override bool TryResolve(AssetKey<IPrimitiveType> resource, DirectoryPath root, string input, out IPrimitiveType value)
        {
            IFile<IPrimitiveType> primitive = this._files.Get<IPrimitiveType>(
                new FilePath(root, input),
                true);

            value = primitive.Value;
            return primitive.Success;
        }
    }
}