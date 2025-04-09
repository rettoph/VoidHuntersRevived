using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Assets.Common;
using Guppy.Core.Assets.Common.AssetTypes;
using VoidHuntersRevived.Domain.Physics.Common;

namespace VoidHuntersRevived.Domain.Physics.AssetTypes
{
    public class BodyTemplateAssetType(IFileService files) : SimpleAssetType<IBodyTemplate>
    {
        private readonly IFileService _files = files;

        public override string Name => nameof(BodyTemplate);

        protected override bool TryResolve(AssetKey<IBodyTemplate> resource, DirectoryPath root, string input, out IBodyTemplate value)
        {
            IFile<IBodyTemplate> template = this._files.Get<IBodyTemplate>(
                new FilePath(root, input),
                true);

            value = template.Value;
            return template.Success;
        }
    }
}