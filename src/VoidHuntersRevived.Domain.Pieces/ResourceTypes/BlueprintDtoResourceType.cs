using Guppy.Attributes;
using Guppy.Files.Enums;
using Guppy.Files;
using Guppy.Resources;
using Guppy.Resources.ResourceTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using Guppy.Files.Services;
using System.Text.Json;
using Guppy.Files.Helpers;

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
