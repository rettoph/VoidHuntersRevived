using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.ResourceTypes;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities.ResourceTypes
{
    internal class EntityTemplateFragmentResourceType(IFileService files) : SimpleResourceType<EntityTemplateFragment>
    {
        private readonly IFileService _files = files;

        public override string Name => "EntityTemplate";

        protected override bool TryResolve(ResourceKey<EntityTemplateFragment> resource, DirectoryPath root, string input, out EntityTemplateFragment value)
        {
            IFile<EntityTemplateFragment> type = this._files.Get<EntityTemplateFragment>(
                new FilePath(root, input),
                true);

            value = type.Value;
            return type.Success;
        }
    }
}