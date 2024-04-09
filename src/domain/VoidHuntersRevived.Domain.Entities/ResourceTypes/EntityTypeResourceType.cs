using Guppy.Attributes;
using Guppy.Files;
using Guppy.Files.Services;
using Guppy.Resources;
using Guppy.Resources.ResourceTypes;
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
