using Guppy.Core.Files.Common;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.ResourceTypes;
using VoidHuntersRevived.Domain.Graphics;
using VoidHuntersRevived.Domain.Graphics.Common;

namespace VoidHuntersRevived.Domain.Pieces.ResourceTypes
{
    public class PrimitiveTypeResourceType(IFileService files) : SimpleResourceType<IPrimitiveType>
    {
        private readonly IFileService _files = files;

        public override string Name => nameof(PrimitiveType);

        protected override bool TryResolve(ResourceKey<IPrimitiveType> resource, DirectoryPath root, string input, out IPrimitiveType value)
        {
            IFile<IPrimitiveType> primitive = this._files.Get<IPrimitiveType>(
                new FilePath(root, input),
                true);

            value = primitive.Value;
            return primitive.Success;
        }
    }
}