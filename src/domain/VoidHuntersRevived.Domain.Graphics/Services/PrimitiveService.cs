using Guppy.Core.Assets.Common.Services;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using IVertexType = Microsoft.Xna.Framework.Graphics.IVertexType;

namespace VoidHuntersRevived.Domain.Graphics.Services
{
    public class PrimitiveService(IAssetService resourceService, IEnumerable<IPrimitive> primitives) : IPrimitiveService
    {
        private readonly IPrimitive[] _primitives = resourceService.GetAll<IPrimitiveType>()
            .Where(x => x.Value is not null)
            .SelectMany(x => x.Value!.Primitives)
            .Concat(primitives)
            .ToArray();

        public IEnumerable<IPrimitive> GetAll()
        {
            return this._primitives;
        }

        public IEnumerable<IPrimitive<TVertex>> GetAll<TVertex>()
            where TVertex : unmanaged, IVertexType
        {
            return this._primitives.OfType<IPrimitive<TVertex>>();
        }

        public IEnumerable<Type> GetAllVertexTypes()
        {
            return this._primitives.Select(x => x.VertexType).Distinct();
        }
    }

    public class PrimitiveService<TVertex> : IPrimitiveService<TVertex>
        where TVertex : unmanaged, IVertexType
    {
        private readonly Dictionary<PrimitiveTypeSequenceGroup, IPrimitive<TVertex>> _grouped;
        private readonly IPrimitive<TVertex>[] _all;

        public PrimitiveService(IAssetService resourceService)
        {
            this._grouped = resourceService.GetAll<IPrimitiveType>()
                .SelectMany(t => t.Value.Primitives.Select(p => (type: t, primitive: p)))
                .Where(x => x.primitive is Primitive<TVertex>)
                .ToDictionary(
                    keySelector: x => new PrimitiveTypeSequenceGroup(x.type.Key, x.primitive.SequenceGroup),
                    elementSelector: x => (IPrimitive<TVertex>)x.primitive);

            this._all = [.. this._grouped.Values];
        }

        public IPrimitive<TVertex>[] GetAll()
        {
            return this._all;
        }

        public IPrimitive<TVertex> GetPrimitiveByTypeAndSequenceGroup(Key<IPrimitiveType> type, PrimitiveSequenceGroupEnum sequenceGroup)
        {
            return this._grouped[new PrimitiveTypeSequenceGroup(type, sequenceGroup)];
        }

        private readonly struct PrimitiveTypeSequenceGroup(Key<IPrimitiveType> type, PrimitiveSequenceGroupEnum sequenceGroup)
        {
            public readonly Key<IPrimitiveType> Type = type;
            public readonly PrimitiveSequenceGroupEnum SequenceGroup = sequenceGroup;

            public override bool Equals(object? obj)
            {
                return obj is PrimitiveTypeSequenceGroup casted &&
                       this.Type == casted.Type &&
                       this.SequenceGroup == casted.SequenceGroup;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(this.Type, this.SequenceGroup);
            }
        }
    }
}