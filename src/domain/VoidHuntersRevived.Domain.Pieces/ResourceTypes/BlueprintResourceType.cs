using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.ResourceTypes;
using VoidHuntersRevived.Domain.Pieces.Common;

namespace VoidHuntersRevived.Domain.Pieces.ResourceTypes
{
    public class BlueprintResourceType(IFileService files) : SimpleResourceType<Blueprint>
    {
        private readonly IFileService _files = files;

        public override string Name => "Blueprint";

        protected override bool TryResolve(ResourceKey<Blueprint> resource, DirectoryPath root, string input, out Blueprint value)
        {
            IFile<Blueprint> blueprint = this._files.Get<Blueprint>(
                new FilePath(root, input),
                true);

            value = blueprint.Value;

            return blueprint.Success;
        }
    }
}