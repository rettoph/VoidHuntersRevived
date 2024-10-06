using Guppy.Core.Resources.Common.Services;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using IVertexType = Microsoft.Xna.Framework.Graphics.IVertexType;

namespace VoidHuntersRevived.Domain.Graphics.Services
{
    public class PrimitiveService(IResourceService resourceService) : IPrimitiveService
    {
        private readonly Primitive[] _primitives = resourceService.GetValues<PrimitiveType>().SelectMany(x => x.Value.Primitives.Value).ToArray();

        public IEnumerable<Primitive> GetAll() => _primitives;

        public IEnumerable<Primitive<TVertex>> GetAll<TVertex>()
            where TVertex : unmanaged, IVertexType => _primitives.OfType<Primitive<TVertex>>();

        public IEnumerable<Type> GetAllVertexTypes() => _primitives.Select(x => x.VertexType).Distinct();
    }

    public class PrimitiveService<TVertex>(IResourceService resourceService) : IPrimitiveService<TVertex>
        where TVertex : unmanaged, IVertexType
    {
        private readonly Dictionary<PrimitiveTypeSequenceGroup, Primitive<TVertex>> _grouped = resourceService.GetValues<PrimitiveType>()
            .SelectMany(t => t.Value.Primitives.Value.Select(p => (type: t, primitive: p)))
            .Where(x => x.primitive is Primitive<TVertex>)
            .ToDictionary(
                keySelector: x => new PrimitiveTypeSequenceGroup(x.type.Resource, x.primitive.SequenceGroup),
                elementSelector: x => (Primitive<TVertex>)x.primitive);

        public Primitive<TVertex> GetPrimitiveByTypeAndSequenceGroup(Key<PrimitiveType> type, PrimitiveSequenceGroupEnum sequenceGroup)
        {
            return _grouped[new PrimitiveTypeSequenceGroup(type, sequenceGroup)];
        }

        private readonly struct PrimitiveTypeSequenceGroup(Key<PrimitiveType> type, PrimitiveSequenceGroupEnum sequenceGroup)
        {
            public readonly Key<PrimitiveType> Type = type;
            public readonly PrimitiveSequenceGroupEnum SequenceGroup = sequenceGroup;

            public override bool Equals(object? obj)
            {
                return obj is PrimitiveTypeSequenceGroup casted &&
                       Type == casted.Type &&
                       SequenceGroup == casted.SequenceGroup;
            }

            public override int GetHashCode() => HashCode.Combine(Type, SequenceGroup);
        }
    }
}
