using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.ResourceTypes;
using VoidHuntersRevived.Domain.Physics.Common;

namespace VoidHuntersRevived.Domain.Physics.ResourceTypes
{
    public class BodyTemplateResourceType(IFileService files) : SimpleResourceType<IBodyTemplate>
    {
        private readonly IFileService _files = files;

        public override string Name => nameof(BodyTemplate);

        protected override bool TryResolve(ResourceKey<IBodyTemplate> resource, DirectoryPath root, string input, out IBodyTemplate value)
        {
            IFile<IBodyTemplate> template = this._files.Get<IBodyTemplate>(
                new FilePath(root, input),
                true);

            value = template.Value;
            return template.Success;
        }
    }
}