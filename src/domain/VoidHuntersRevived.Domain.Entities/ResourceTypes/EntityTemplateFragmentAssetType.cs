using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Assets.Common;
using Guppy.Core.Assets.Common.AssetTypes;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities.AssetTypes
{
    internal class EntityTemplateFragmentAssetType(IFileService files) : SimpleAssetType<EntityTemplateFragment>
    {
        private readonly IFileService _files = files;

        public override string Name => "EntityTemplate";

        protected override bool TryResolve(AssetKey<EntityTemplateFragment> resource, DirectoryPath root, string input, out EntityTemplateFragment value)
        {
            IFile<EntityTemplateFragment> type = this._files.Get<EntityTemplateFragment>(
                new FilePath(root, input),
                true);

            value = type.Value;
            return type.Success;
        }
    }
}