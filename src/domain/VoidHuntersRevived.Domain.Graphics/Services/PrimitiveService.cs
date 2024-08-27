using Guppy.Core.Common;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Exceptions;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Factories;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Graphics.Services
{
    public class PrimitiveService : StrategyEngine, IPrimitiveService, IQueryingEntitiesEngine
    {
        private readonly IPrimitive[] _primitives;
        private readonly Dictionary<Key<IEntityType>, IPrimitive> _primitivesByEntityTypeKey;
        private readonly Dictionary<Type, IPrimitive[]> _primitivesByVertexType;

        public EntitiesDB entitiesDB
        {
            set
            {
                foreach (IPrimitive primitive in _primitives)
                {
                    primitive.EntitiesDb = value;
                }
            }
        }

        public PrimitiveService(IFiltered<IPrimitiveFactory> primitiveFactories)
        {
            _primitives = primitiveFactories.SelectMany(x => x.BuildPrimitives()).ToArray();
            _primitivesByEntityTypeKey = _primitives.Where(x => x.EntityTypeKey is not null).ToDictionary(x => x.EntityTypeKey!.Value, x => x);
            _primitivesByVertexType = _primitives.GroupBy(x => x.VertexType).ToDictionary(x => x.Key, x => x.ToArray());

            // Validate primitives
            // Rule 1 - Primitives not attached to an entity type must have a unique VertexType
            // The VertexType is the primary key to locate the primitive rather than entity type
            foreach (IPrimitive primitive in _primitives.Where(x => x.EntityTypeKey is null))
            {
                IPrimitive[] primitiveTypes = _primitivesByVertexType[primitive.VertexType];
                if (_primitivesByVertexType[primitive.VertexType].Length > 1)
                {
                    throw new DuplicatePrimitiveVertexTypeException(primitive.VertexType, primitiveTypes);
                }
            }
        }

        public IPrimitive GetByEntityTypeKey(Key<IEntityType> entityTypeKey)
        {
            return _primitivesByEntityTypeKey[entityTypeKey];
        }

        public IPrimitive<TVertex> GetByEntityTypeKey<TVertex>(Key<IEntityType> entityTypeKey)
            where TVertex : unmanaged, IVertexType
        {
            return (IPrimitive<TVertex>)_primitivesByEntityTypeKey[entityTypeKey];
        }

        public IEnumerable<IPrimitive> GetAll()
        {
            return _primitives;
        }

        public IReadOnlyDictionary<Type, IPrimitive[]> GetAllByVertexType()
        {
            return _primitivesByVertexType;
        }

        public IPrimitive GetByVertexType(Type vertexType)
        {
            ThrowIf.Type.IsNotAssignableFrom<IVertexType>(vertexType);
            ThrowIf.Type.IsNotUnmanagedStruct(vertexType);

            IPrimitive[] primitives = _primitivesByVertexType[vertexType];
            if (primitives.Length != 1)
            {
                throw new DuplicatePrimitiveVertexTypeException(vertexType, primitives);
            }

            return primitives[0];
        }

        public IPrimitive<TVertex> GetByVertexType<TVertex>()
            where TVertex : unmanaged, IVertexType
        {
            IPrimitive[] primitives = _primitivesByVertexType[typeof(TVertex)];
            if (primitives.Length != 1)
            {
                throw new DuplicatePrimitiveVertexTypeException(typeof(TVertex), primitives);
            }

            return (IPrimitive<TVertex>)primitives[0];
        }

        public IEnumerable<IPrimitive> GetAllByVertexType(Type vertexType)
        {
            ThrowIf.Type.IsNotAssignableFrom<IVertexType>(vertexType);
            ThrowIf.Type.IsNotUnmanagedStruct(vertexType);

            return _primitivesByVertexType[vertexType];
        }

        public IEnumerable<IPrimitive<TVertex>> GetAllByVertexType<TVertex>() where TVertex : unmanaged, IVertexType
        {
            return _primitivesByVertexType[typeof(TVertex)].OfType<IPrimitive<TVertex>>();
        }
    }
}
