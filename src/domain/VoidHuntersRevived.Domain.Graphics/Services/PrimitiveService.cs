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
        private readonly IPrimitive[] _primitives = resourceService.GetValues<IPrimitiveType>().SelectMany(x => x.Value.Primitives).ToArray();

        public IEnumerable<IPrimitive> GetAll() => _primitives;

        public IEnumerable<IPrimitive<TVertex>> GetAll<TVertex>()
            where TVertex : unmanaged, IVertexType => _primitives.OfType<IPrimitive<TVertex>>();

        public IEnumerable<Type> GetAllVertexTypes() => _primitives.Select(x => x.VertexType).Distinct();
    }

    public class PrimitiveService<TVertex> : IPrimitiveService<TVertex>
        where TVertex : unmanaged, IVertexType
    {
        private readonly Dictionary<PrimitiveTypeSequenceGroup, IPrimitive<TVertex>> _grouped;
        private readonly IPrimitive<TVertex>[] _all;

        public PrimitiveService(IResourceService resourceService)
        {
            _grouped = resourceService.GetValues<IPrimitiveType>()
                .SelectMany(t => t.Value.Primitives.Select(p => (type: t, primitive: p)))
                .Where(x => x.primitive is Primitive<TVertex>)
                .ToDictionary(
                    keySelector: x => new PrimitiveTypeSequenceGroup(x.type.Resource, x.primitive.SequenceGroup),
                    elementSelector: x => (IPrimitive<TVertex>)x.primitive);

            _all = [.. _grouped.Values];
        }

        public IPrimitive<TVertex>[] GetAll()
        {
            return _all;
        }

        public IPrimitive<TVertex> GetPrimitiveByTypeAndSequenceGroup(Key<IPrimitiveType> type, PrimitiveSequenceGroupEnum sequenceGroup)
        {
            return _grouped[new PrimitiveTypeSequenceGroup(type, sequenceGroup)];
        }

        private readonly struct PrimitiveTypeSequenceGroup(Key<IPrimitiveType> type, PrimitiveSequenceGroupEnum sequenceGroup)
        {
            public readonly Key<IPrimitiveType> Type = type;
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
