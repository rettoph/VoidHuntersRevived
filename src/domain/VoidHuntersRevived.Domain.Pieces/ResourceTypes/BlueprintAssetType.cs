using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Assets.Common;
using Guppy.Core.Assets.Common.AssetTypes;
using VoidHuntersRevived.Domain.Pieces.Common;

namespace VoidHuntersRevived.Domain.Pieces.AssetTypes
{
    public class BlueprintAssetType(IFileService files) : SimpleAssetType<Blueprint>
    {
        private readonly IFileService _files = files;

        public override string Name => "Blueprint";

        protected override bool TryResolve(AssetKey<Blueprint> resource, DirectoryPath root, string input, out Blueprint value)
        {
            IFile<Blueprint> blueprint = this._files.Get<Blueprint>(
                new FilePath(root, input),
                true);

            value = blueprint.Value;

            return blueprint.Success;
        }
    }
}