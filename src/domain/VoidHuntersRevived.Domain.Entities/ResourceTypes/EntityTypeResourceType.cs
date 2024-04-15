using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Resources;
using Guppy.Core.Resources.ResourceTypes;
using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.ResourceTypes
{
    [AutoLoad]
    internal class EntityTypeResourceType : SimpleResourceType<IEntityType>
    {
        private readonly IFileService _files;

        public override string Name => "EntityType";

        public EntityTypeResourceType(IFileService files)
        {
            _files = files;
        }

        protected override bool TryResolve(Resource<IEntityType> resource, DirectoryLocation root, string input, out IEntityType value)
        {
            IFile<IEntityType> type = _files.Get<IEntityType>(
                new FileLocation(root, input),
                true);

            value = type.Value;
            return type.Success;
        }
    }
}
