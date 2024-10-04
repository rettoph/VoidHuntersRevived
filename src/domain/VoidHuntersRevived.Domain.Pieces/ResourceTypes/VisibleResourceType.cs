using Guppy.Core.Common.Attributes;
using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.ResourceTypes;
using VoidHuntersRevived.Domain.Pieces.Common.Resources;

namespace VoidHuntersRevived.Domain.Pieces.ResourceTypes
{
    [AutoLoad]
    public class VisibleResourceType(IFileService files) : SimpleResourceType<Visible>
    {
        private readonly IFileService _files = files;

        public override string Name => "Visible";

        protected override bool TryResolve(Resource<Visible> resource, DirectoryLocation root, string input, out Visible value)
        {
            IFile<Visible> visible = _files.Get<Visible>(
                new FileLocation(root, input),
                true);

            value = visible.Value;
            return visible.Success;
        }
    }
}
