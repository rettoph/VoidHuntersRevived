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
    internal class EntityTypeConfigurationResourceType(IFileService files) : ResourceType<EntityTypeConfiguration>
    {
        private readonly IFileService _files = files;

        public override string Name => "EntityType";

        protected override bool TryGetResolver(Resource<EntityTypeConfiguration> resource, DirectoryLocation root, ref JsonElement json, [MaybeNullWhen(false)] out ResourceResolver<EntityTypeConfiguration> resolver)
        {
            string input = json.GetString() ?? string.Empty;

            IFile<ResourceResolver<EntityTypeConfiguration>> type = _files.Get<ResourceResolver<EntityTypeConfiguration>>(
                new FileLocation(root, input),
                true);

            resolver = type.Value;
            return type.Success;
        }
    }
}
