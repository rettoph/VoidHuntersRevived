using Guppy.Core.Common;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Factories;

namespace VoidHuntersRevived.Domain.Graphics.Services
{
    public class PrimitiveService : IPrimitiveService
    {
        private readonly IPrimitive[] _primitives;
        private readonly Dictionary<IKey<IEntityType>, IPrimitive> _primitivesByEntityTypeKey;


        public PrimitiveService(IFiltered<IPrimitiveFactory> primitiveFactories)
        {
            _primitives = primitiveFactories.SelectMany(x => x.BuildPrimitives()).ToArray();
            _primitivesByEntityTypeKey = _primitives.ToDictionary(x => x.EntityTypeKey, x => x);
        }

        public IPrimitive GetPrimitiveByEntityTypeKey(IKey<IEntityType> entityTypeKey)
        {
            return _primitivesByEntityTypeKey[entityTypeKey];
        }

        public IPrimitive<TVertex> GetPrimitiveByEntityTypeKey<TVertex>(IKey<IEntityType> entityTypeKey)
            where TVertex : unmanaged, IVertexType
        {
            return (IPrimitive<TVertex>)_primitivesByEntityTypeKey[entityTypeKey];
        }
    }
}
