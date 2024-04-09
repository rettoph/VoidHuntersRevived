using Guppy.Attributes;
using Guppy.Files;
using Guppy.Files.Services;
using Guppy.Resources;
using Guppy.Resources.ResourceTypes;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities.ResourceTypes
{
    [AutoLoad]
    internal class EntityContextResourceType : SimpleResourceType<EntityContext>
    {
        private readonly IFileService _files;

        public override string Name => "EntityType";

        public EntityContextResourceType(IFileService files)
        {
            _files = files;
        }

        protected override bool TryResolve(Resource<EntityContext> resource, DirectoryLocation root, string input, out EntityContext value)
        {
            IFile<EntityContext> piece = _files.Get<EntityContext>(
                new FileLocation(root, input),
                true);

            value = piece.Value;
            return piece.Success;
        }
    }
}
