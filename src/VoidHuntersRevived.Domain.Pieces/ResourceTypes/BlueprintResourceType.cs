using Guppy.Attributes;
using Guppy.Files;
using Guppy.Files.Enums;
using Guppy.Files.Helpers;
using Guppy.Files.Services;
using Guppy.Resources;
using Guppy.Resources.ResourceTypes;
using VoidHuntersRevived.Common.Pieces;

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

        protected override bool TryResolve(Resource<Blueprint> resource, string root, string input, out Blueprint value)
        {
            IFile<Blueprint> blueprint = _files.Get<Blueprint>(
                FileType.Source,
                DirectoryHelper.Combine(root, input),
                true);

            value = blueprint.Value;

            return blueprint.Success;
        }
    }
}
