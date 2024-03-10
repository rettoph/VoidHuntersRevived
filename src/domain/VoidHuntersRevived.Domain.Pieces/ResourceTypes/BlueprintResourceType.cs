using Guppy.Attributes;
using Guppy.Files;
using Guppy.Files.Services;
using Guppy.Resources;
using Guppy.Resources.ResourceTypes;
using VoidHuntersRevived.Domain.Pieces.Common;

namespace VoidHuntersRevived.Domain.Pieces.ResourceTypes
{
    [AutoLoad]
    internal class BlueprintResourceType : SimpleResourceType<Blueprint>
    {
        private readonly IFileService _files;

        public override string Name => "Blueprint";

        public BlueprintResourceType(IFileService files)
        {
            _files = files;
        }

        protected override bool TryResolve(Resource<Blueprint> resource, DirectoryLocation root, string input, out Blueprint value)
        {
            IFile<Blueprint> blueprint = _files.Get<Blueprint>(
                new FileLocation(root, input),
                true);

            value = blueprint.Value;

            return blueprint.Success;
        }
    }
}
