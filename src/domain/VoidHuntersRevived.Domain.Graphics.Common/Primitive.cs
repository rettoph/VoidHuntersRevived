using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;

namespace VoidHuntersRevived.Domain.Graphics.Common
{
    public class Primitive<TVertex> : IPrimitive<TVertex>
        where TVertex : unmanaged, IVertexType
    {
        private readonly Dictionary<PrimitiveGroupEnum, IVertexBuffer<TVertex>> _vertexBuffers;

        public IKey<IEntityType>? EntityTypeKey { get; }

        public Type VertexType { get; }

        public PrimitiveGroupEnum[] Groups { get; }

        public EntitiesDB EntitiesDb
        {
            set
            {
                foreach (IVertexBuffer<TVertex> vertexBuffer in _vertexBuffers.Values)
                {
                    vertexBuffer.EntitiesDb = value;
                }
            }
        }

        public Primitive(
            GraphicsDevice graphics,
            IKey<IEntityType>? entityTypeKey,
            PrimitiveGroupEnum[] primitiveGroups,
            BufferContext[] bufferContexts)
        {
            this.EntityTypeKey = entityTypeKey;
            this.VertexType = typeof(TVertex);
            this.Groups = primitiveGroups;

            _vertexBuffers = primitiveGroups.ToDictionary(
                keySelector: x => x,
                elementSelector: x => (IVertexBuffer<TVertex>)new VertexBuffer<TVertex>(
                    graphics: graphics,
                    staticBuffers: bufferContexts.Select(x => x.BuildVertexBuffer(graphics)).ToArray(),
                    indexBuffers: bufferContexts.Select(x => x.BuildIndexBuffer(graphics)).ToArray(),
                    primitiveTypes: bufferContexts.Select(x => x.PrimitiveType).ToArray()));
        }

        public IVertexBuffer<TVertex> GetVertexBuffer(PrimitiveGroupEnum primitiveGroup)
        {
            return _vertexBuffers[primitiveGroup];
        }

        public IEnumerable<KeyValuePair<PrimitiveGroupEnum, IVertexBuffer<TVertex>>> GetAllVertexBuffers()
        {
            return _vertexBuffers;
        }
    }
}
