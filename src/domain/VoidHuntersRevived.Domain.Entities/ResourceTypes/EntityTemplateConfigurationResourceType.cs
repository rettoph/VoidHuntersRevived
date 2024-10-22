using Guppy.Core.Common.Attributes;
using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.ResourceTypes;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities.ResourceTypes
{
    [AutoLoad]
    internal class EntityTemplateConfigurationResourceType(IFileService files) : ResourceType<EntityTemplateConfiguration>
    {
        private readonly IFileService _files = files;

        public override string Name => "EntityTemplate";

        protected override bool TryGetResolver(Resource<EntityTemplateConfiguration> resource, DirectoryLocation root, ref JsonElement json, [MaybeNullWhen(false)] out ResourceResolver<EntityTemplateConfiguration> resolver)
        {
            string input = json.GetString() ?? string.Empty;

            IFile<ResourceResolver<EntityTemplateConfiguration>> type = _files.Get<ResourceResolver<EntityTemplateConfiguration>>(
                new FileLocation(root, input),
                true);

            resolver = type.Value;
            return type.Success;
        }
    }
}
