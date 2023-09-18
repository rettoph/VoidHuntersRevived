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

namespace VoidHuntersRevived.Domain.Pieces.ResourceTypes
{
    [AutoLoad]
    internal class BlueprintResourceType : ResourceType<Blueprint>
    {
        private readonly IFileService _files;

        public BlueprintResourceType(IFileService files)
        {
            _files = files;
        }

        protected override bool TryResolve(Resource<Blueprint> resource, string input, string root, out Blueprint value)
        {
            IFile<Blueprint> blueprint = _files.Get<Blueprint>(
                FileType.Source,
                Path.Combine(root, input),
                true);

            value = blueprint.Value;

            return blueprint.Success;
        }
    }
}
