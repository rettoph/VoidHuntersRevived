using Guppy.Core.Common.Attributes;
using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.ResourceTypes;
using VoidHuntersRevived.Domain.Graphics;
using VoidHuntersRevived.Domain.Graphics.Common;

namespace VoidHuntersRevived.Domain.Pieces.ResourceTypes
{
    [AutoLoad]
    public class PrimitiveTypeResourceType(IFileService files) : SimpleResourceType<IPrimitiveType>
    {
        private readonly IFileService _files = files;

        public override string Name => nameof(PrimitiveType);

        protected override bool TryResolve(Resource<IPrimitiveType> resource, DirectoryLocation root, string input, out IPrimitiveType value)
        {
            IFile<IPrimitiveType> primitive = _files.Get<IPrimitiveType>(
                new FileLocation(root, input),
                true);

            value = primitive.Value;
            return primitive.Success;
        }
    }
}
