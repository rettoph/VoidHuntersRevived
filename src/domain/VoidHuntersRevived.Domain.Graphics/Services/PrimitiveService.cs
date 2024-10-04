using Guppy.Core.Common;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common.Services;

namespace VoidHuntersRevived.Domain.Graphics.Services
{
    public class PrimitiveService(
        IFiltered<IPrimitive> primitives,
        IFiltered<IPrimitiveProvider> providers) : IPrimitiveService, IEngineProvider
    {
        private readonly IPrimitive[] _primitives = [.. primitives, .. providers.SelectMany(x => x.GetPrimitives())];

        public IEnumerable<IPrimitive> GetAll() => _primitives;

        public IEnumerable<IPrimitive<TVertex>> GetAll<TVertex>()
            where TVertex : unmanaged, IVertexType => _primitives.OfType<IPrimitive<TVertex>>();

        public IEnumerable<Type> GetAllVertexTypes() => _primitives.Select(x => x.VertexType).Distinct();

        IEnumerable<IEngine> IEngineProvider.GetEngines() => _primitives.OfType<IEngine>();
    }

    public class PrimitiveService<TVertex>(IPrimitiveService primitiveService) : IPrimitiveService<TVertex>
        where TVertex : unmanaged, IVertexType
    {
        private readonly struct PrimitiveTypeSequenceGroup(Key<IPrimitive> type, PrimitiveSequenceGroupEnum sequenceGroup)
        {
            public readonly Key<IPrimitive> Type = type;
            public readonly PrimitiveSequenceGroupEnum SequenceGroup = sequenceGroup;

            public override bool Equals(object? obj)
            {
                return obj is PrimitiveTypeSequenceGroup casted &&
                       Type == casted.Type &&
                       SequenceGroup == casted.SequenceGroup;
            }

            public override int GetHashCode() => HashCode.Combine(Type, SequenceGroup);
        }

        private readonly Dictionary<PrimitiveTypeSequenceGroup, IPrimitive<TVertex>> _grouped = primitiveService.GetAll<TVertex>()
            .ToDictionary(
                keySelector: x => new PrimitiveTypeSequenceGroup(x.Type, x.SequenceGroup),
                elementSelector: x => x);

        public IPrimitive<TVertex> GetPrimitiveByTypeAndSequenceGroup(Key<IPrimitive> type, PrimitiveSequenceGroupEnum sequenceGroup)
        {
            return _grouped[new PrimitiveTypeSequenceGroup(type, sequenceGroup)];
        }
    }
}
