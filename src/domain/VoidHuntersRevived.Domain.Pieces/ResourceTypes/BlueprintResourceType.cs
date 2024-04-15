using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.ResourceTypes;
using Guppy.Core.Common.Attributes;
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
