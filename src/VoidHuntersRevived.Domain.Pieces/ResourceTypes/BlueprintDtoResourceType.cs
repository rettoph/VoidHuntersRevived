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
    internal class BlueprintDtoResourceType : SimpleResourceType<BlueprintDto>
    {
        private readonly IFileService _files;

        public override string Name => "Blueprint";

        public BlueprintDtoResourceType(IFileService files)
        {
            _files = files;
        }

        protected override bool TryResolve(Resource<BlueprintDto> resource, string root, string input, out BlueprintDto value)
        {
            IFile<BlueprintDto> blueprint = _files.Get<BlueprintDto>(
                FileType.Source,
                DirectoryHelper.Combine(root, input),
                true);

            value = blueprint.Value;

            return blueprint.Success;
        }
    }
}
