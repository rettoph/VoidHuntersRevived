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
    internal class EntityTypeResourceType : ResourceType<IEntityType>
    {
        private readonly IFileService _files;

        public override string Name => "EntityType";

        public EntityTypeResourceType(IFileService files)
        {
            _files = files;
        }

        protected override bool TryGetResolver(Resource<IEntityType> resource, DirectoryLocation root, ref JsonElement json, [MaybeNullWhen(false)] out ResourceResolver<IEntityType> resolver)
        {
            string input = json.GetString() ?? string.Empty;

            IFile<ResourceResolver<IEntityType>> type = _files.Get<ResourceResolver<IEntityType>>(
                new FileLocation(root, input),
                true);

            resolver = type.Value;
            return type.Success;
        }
    }
}
