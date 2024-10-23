using Guppy.Core.Common.Attributes;
using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.ResourceTypes;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities.ResourceTypes
{
    [AutoLoad]
    internal class EntityTemplateConfigurationResourceType(IFileService files) : SimpleResourceType<EntityTemplateConfiguration>
    {
        private readonly IFileService _files = files;

        public override string Name => "EntityTemplate";

        protected override bool TryResolve(Resource<EntityTemplateConfiguration> resource, DirectoryLocation root, string input, out EntityTemplateConfiguration value)
        {
            IFile<EntityTemplateConfiguration> type = _files.Get<EntityTemplateConfiguration>(
                new FileLocation(root, input),
                true);

            value = type.Value;
            return type.Success;
        }
    }
}
