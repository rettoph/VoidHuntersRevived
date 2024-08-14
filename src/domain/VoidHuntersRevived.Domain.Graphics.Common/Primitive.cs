using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;

namespace VoidHuntersRevived.Domain.Graphics.Common
{
    public class Primitive<TVertex> : IPrimitive<TVertex>
        where TVertex : unmanaged, IVertexType
    {
        private readonly Dictionary<PrimitiveGroupEnum, IVertexProvider<TVertex>> _vertexProviders;

        public IKey<IEntityType> EntityTypeKey { get; }

        public Type VertexType { get; }

        public Primitive(
            GraphicsDevice graphics,
            IKey<IEntityType> entityTypeKey,
            PrimitiveGroupEnum[] primitiveGroups,
            BufferContext[] bufferContexts)
        {
            this.EntityTypeKey = entityTypeKey;
            this.VertexType = typeof(TVertex);

            _vertexProviders = primitiveGroups.ToDictionary(
                keySelector: x => x,
                elementSelector: x => (IVertexProvider<TVertex>)new VertexProvider<TVertex>(
                    graphics: graphics,
                    staticBuffers: bufferContexts.Select(x => x.BuildVertexBuffer(graphics)).ToArray(),
                    indexBuffers: bufferContexts.Select(x => x.BuildIndexBuffer(graphics)).ToArray(),
                    primitiveTypes: bufferContexts.Select(x => x.PrimitiveType).ToArray()));
        }

        public IVertexProvider<TVertex> GetVertexProvider(PrimitiveGroupEnum primitiveGroup)
        {
            return _vertexProviders[primitiveGroup];
        }

        public IEnumerable<KeyValuePair<PrimitiveGroupEnum, IVertexProvider<TVertex>>> GetAllVertexProviders()
        {
            return _vertexProviders;
        }
    }
}
